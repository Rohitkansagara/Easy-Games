using AutoMapper;
using EasyGames.Class;
using EasyGames.Class.DATA;
using EasyGames.Services.ExtensionMethod;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyGames.Services.Services
{
    public interface IBaseService<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(long id);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(long id);

        // DTO CRUD
        Task<TEntityDto> AddAsync<TEntityDto>(TEntityDto dto)
            where TEntityDto : IDocumentId;

        Task<TEntityDto> UpdateAsync<TEntityDto>(TEntityDto dto)
            where TEntityDto : IDocumentId, IRowVersion;

        Task<TEntityDto> GetAsync<TEntityDto>(long id)
            where TEntityDto : IDocumentId;
    }

    public class BaseService<T> : IBaseService<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;
        protected readonly IMapper _mapper;
        protected readonly ICurrentUserInfo _currentUserInfo;

        public BaseService(
            ApplicationDbContext context,
            IMapper mapper,
            ICurrentUserInfo currentUserInfo)
        {
            _context = context;
            _dbSet = _context.Set<T>();
            _mapper = mapper;
            _currentUserInfo = currentUserInfo;
        }

        #region Audit Helpers

        protected virtual void SetRecordCreatedInfo(object record)
        {
            if (record is IRecordCreatedInfo createdInfo)
            {
                createdInfo.CreatedOn = DateTimeOffset.UtcNow;
                createdInfo.CreatedById =
                    _currentUserInfo.IsAuthenticated ? _currentUserInfo.UserId : null;
            }
        }

        protected virtual void SetRecordModifiedInfo(
            object record,
            byte[]? incomingRowVersion = null)
        {
            if (record is IRecordModifiedInfo modifiedInfo)
            {
                modifiedInfo.ModifiedOn = DateTimeOffset.UtcNow;
                modifiedInfo.ModifiedById =
                    _currentUserInfo.IsAuthenticated ? _currentUserInfo.UserId : null;
            }

            // ✅ RowVersion concurrency check
            if (record is IRowVersion rowVersionEntity && incomingRowVersion != null)
            {
                _context.Entry(rowVersionEntity)
                    .Property(nameof(IRowVersion.RowVersion))
                    .OriginalValue = incomingRowVersion;
            }
        }

        #endregion

        #region Entity CRUD

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(long id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> AddAsync(T entity)
        {
            SetRecordCreatedInfo(entity);
            SetRecordModifiedInfo(entity);

            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            SetRecordModifiedInfo(entity);

            _dbSet.Update(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
                return false;

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region DTO CRUD (WITH ROW VERSION)

        public async Task<TEntityDto> AddAsync<TEntityDto>(TEntityDto dto)
            where TEntityDto : IDocumentId
        {
            var entity = _mapper.Map<T>(dto);

            SetRecordCreatedInfo(entity);
            SetRecordModifiedInfo(entity);

            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<TEntityDto>(entity);
        }

        public async Task<TEntityDto> GetAsync<TEntityDto>(long id)
            where TEntityDto : IDocumentId
        {
            var entity = await _dbSet.FindAsync(id);

            if (entity == null)
                throw new KeyNotFoundException("Record not found");

            return _mapper.Map<TEntityDto>(entity);
        }

        public async Task<TEntityDto> UpdateAsync<TEntityDto>(TEntityDto dto)
            where TEntityDto : IDocumentId, IRowVersion
        {
            var entity = await _dbSet.FindAsync(dto.Id);

            if (entity == null)
                throw new KeyNotFoundException("Record not found");

            // Map DTO → Entity
            _mapper.Map(dto, entity);

            // Set modified info + concurrency token
            SetRecordModifiedInfo(entity, dto.RowVersion);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "This record was modified by another user. Please refresh and try again.");
            }

            return _mapper.Map<TEntityDto>(entity);
        }

        #endregion
    }
}
