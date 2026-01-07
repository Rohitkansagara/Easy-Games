using System;

namespace EasyGames.Class.Enum
{
    [Flags]
    public enum EnumPermissions
    {
        None = 0,

        // -------------------------------------------------------------
        // BASE PERMISSIONS
        // -------------------------------------------------------------
        ViewAccess = 1 << 0,   // 1
        UpdateAccess = 1 << 1,   // 2
        CreateAccess = 1 << 2,   // 4
        DeleteAccess = 1 << 3,   // 8
        ApproveAccess = 1 << 4,   // 16


        // -------------------------------------------------------------
        // TWO-PERMISSION COMBINATIONS
        // -------------------------------------------------------------
        ViewUpdateAccess = ViewAccess | UpdateAccess,        // 1 + 2 = 3
        ViewCreateAccess = ViewAccess | CreateAccess,        // 1 + 4 = 5
        ViewDeleteAccess = ViewAccess | DeleteAccess,        // 1 + 8 = 9
        ViewApproveAccess = ViewAccess | ApproveAccess,       // 1 + 16 = 17

        UpdateCreateAccess = UpdateAccess | CreateAccess,    // 2 + 4 = 6
        UpdateDeleteAccess = UpdateAccess | DeleteAccess,    // 2 + 8 = 10
        UpdateApproveAccess = UpdateAccess | ApproveAccess,   // 2 + 16 = 18

        CreateDeleteAccess = CreateAccess | DeleteAccess,    // 4 + 8 = 12
        CreateApproveAccess = CreateAccess | ApproveAccess,   // 4 + 16 = 20

        DeleteApproveAccess = DeleteAccess | ApproveAccess,   // 8 + 16 = 24


        // -------------------------------------------------------------
        // THREE-PERMISSION COMBINATIONS
        // -------------------------------------------------------------
        ViewUpdateCreateAccess = ViewAccess | UpdateAccess | CreateAccess,                    // 1+2+4 = 7
        ViewUpdateDeleteAccess = ViewAccess | UpdateAccess | DeleteAccess,                    // 1+2+8 = 11
        ViewUpdateApproveAccess = ViewAccess | UpdateAccess | ApproveAccess,                  // 1+2+16 = 19

        ViewCreateDeleteAccess = ViewAccess | CreateAccess | DeleteAccess,                    // 1+4+8 = 13
        ViewCreateApproveAccess = ViewAccess | CreateAccess | ApproveAccess,                  // 1+4+16 = 21

        ViewDeleteApproveAccess = ViewAccess | DeleteAccess | ApproveAccess,                  // 1+8+16 = 25

        UpdateCreateDeleteAccess = UpdateAccess | CreateAccess | DeleteAccess,                // 2+4+8 = 14
        UpdateCreateApproveAccess = UpdateAccess | CreateAccess | ApproveAccess,              // 2+4+16 = 22

        UpdateDeleteApproveAccess = UpdateAccess | DeleteAccess | ApproveAccess,              // 2+8+16 = 26

        CreateDeleteApproveAccess = CreateAccess | DeleteAccess | ApproveAccess,              // 4+8+16 = 28


        // -------------------------------------------------------------
        // FOUR-PERMISSION COMBINATIONS
        // -------------------------------------------------------------
        CURDAccess = ViewAccess | UpdateAccess | CreateAccess | DeleteAccess,                 // 1+2+4+8 = 15

        ViewUpdateCreateDeleteAccess = ViewAccess | UpdateAccess | CreateAccess | DeleteAccess,     // = 15
        ViewUpdateCreateApproveAccess = ViewAccess | UpdateAccess | CreateAccess | ApproveAccess,   // 1+2+4+16 = 23
        ViewUpdateDeleteApproveAccess = ViewAccess | UpdateAccess | DeleteAccess | ApproveAccess,   // 1+2+8+16 = 27
        ViewCreateDeleteApproveAccess = ViewAccess | CreateAccess | DeleteAccess | ApproveAccess,   // 1+4+8+16 = 29
        UpdateCreateDeleteApproveAccess = UpdateAccess | CreateAccess | DeleteAccess | ApproveAccess, // 2+4+8+16 = 30


        // -------------------------------------------------------------
        // FIVE-PERMISSION COMBINATION (FULL)
        // -------------------------------------------------------------
        FullAccess = ViewAccess | UpdateAccess | CreateAccess | DeleteAccess | ApproveAccess,  // 1+2+4+8+16 = 31

        // Alias for admins
        AdminAccess = FullAccess
    }
}
