namespace Diary.Domain.Enum;

public enum ErrorCodes
{
    //0 - 9 Reports
    ReportsNotFound = 0,
    ReportNotFound = 1,
    ReportIsAlreadyExists = 2,
    
    
    //11-20 User
    UserNotFound = 11,
    UserAlreadyExists = 12,
    UserUnautorizedAcess = 13,
    UserAlreadyHasThisRole = 14,
    
    PasswordsNotMatch = 21,
    PasswordIsWrong = 22,
    InvalidClientRequest = 23,
    
    RoleAlreadyExists = 31,
    RoleDoesNotExists = 32, 
    
    InternalServerError = 10,
}