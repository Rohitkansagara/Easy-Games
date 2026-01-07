using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyGames.Class.Enum
{
    public enum EnumEntityEvents
    {
        None = 0,
        Unknown = 1,

        // ----------------------------
        // LIST OPERATIONS
        // ----------------------------
        COMMON_LIST = 2,
        COMMON_LIST_EXCEPTION = 3,

        // ----------------------------
        // GET OPERATIONS
        // ----------------------------
        NOT_FOUND = 4,
        COMMON_GET = 5,
        COMMON_GET_EXCEPTION = 6,

        // ----------------------------
        // CREATE OPERATIONS
        // ----------------------------
        COMMON_CREATE = 7,
        COMMON_CREATE_EXCEPTION = 8,

        // ----------------------------
        // UPDATE OPERATIONS
        // ----------------------------
        COMMON_UPDATE = 9,
        COMMON_UPDATE_EXCEPTION = 10,

        // ----------------------------
        // DELETE OPERATIONS
        // ----------------------------
        COMMON_DELETE = 11,
        COMMON_DELETE_EXCEPTION = 12,

        // ----------------------------
        // VALIDATION ERRORS
        // ----------------------------
        VALIDATION_FAILED = 13,

        // ----------------------------
        // PERMISSION / AUTH ISSUES
        // ----------------------------
        UNAUTHORIZED = 14,
        FORBIDDEN = 15,

        // ----------------------------
        // INTERNAL SERVER ERROR
        // ----------------------------
        SERVER_ERROR = 16
    }

}
