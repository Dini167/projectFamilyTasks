namespace projectFamilyTasks.Application.Common.Constants;

public static class ErrorMessages
{
    public const string Unexpected = "אירעה שגיאה בלתי צפויה. יש לנסות שוב מאוחר יותר.";

    public const string InvalidData = "הנתונים שנשלחו אינם תקינים.";

    public const string InvalidFormat = "הנתונים שנשלחו אינם בפורמט תקין.";

    public const string LoginRequired = "יש להתחבר כדי לבצע פעולה זו.";

    public const string SessionExpired = "תוקף ההתחברות פג. יש להתחבר מחדש.";

    public const string Forbidden = "אין הרשאה לבצע פעולה זו.";

    public const string ConcurrencyConflict =
        "הנתונים שונו בינתיים על ידי משתמש אחר, למשל אח שתפס את אותה משימה. יש לרענן ולנסות שוב.";

    public const string LoginFailed = "שם המשתמש או הסיסמה שגויים.";
    public const string UserNameTaken = "שם המשתמש כבר תפוס. יש לבחור שם משתמש אחר.";
    public const string ChildPendingApproval = "החשבון ממתין לאישור של אחד ההורים. אפשר להתחבר לאחר האישור.";
    public const string ParentsPasswordWrong = "סיסמת ההורים שגויה.";
    public const string CurrentPasswordWrong = "הסיסמה הנוכחית שגויה.";
    public const string ParentAccessMissing = "הגדרות ההורים טרם אותחלו. יש לפנות למנהל המערכת.";
    public const string UserNotFound = "המשתמש לא נמצא. יש להתחבר מחדש.";

    public const string DuplicateValue = "הערך שנבחר כבר קיים במערכת.";
    public const string InUse = "לא ניתן למחוק פריט שמשמש במקום אחר במערכת.";

    public const string ChildNotFound = "הילד לא נמצא.";

    public const string TaskNotFound = "המשימה לא נמצאה.";
    public const string CategoryNotFound = "אחת מהקטגוריות שנבחרו אינה קיימת.";
    public const string TaskHasAssignments = "לא ניתן למחוק משימה שילד כבר לקח או ביצע.";

    public const string RoundNotFound = "הסבב לא נמצא.";
    public const string NoCurrentRound = "אין כרגע סבב פתוח או פעיל.";
    public const string RoundAlreadyOpen = "כבר קיים סבב פתוח. יש לסגור אותו לפני פתיחת סבב חדש.";
    public const string NoTasksForRound = "אין משימות במערכת. יש להוסיף משימות לפני פתיחת סבב.";
    public const string EndDateInPast = "תאריך הסיום חייב להיות בעתיד.";
    public const string RoundTransitionInvalid = "לא ניתן לשנות את מצב הסבב בצורה הזו.";
    public const string NoOpenSelection = "אין כרגע סבב שפתוח לבחירת משימות.";
    public const string SelectionClosed = "הבחירה בסבב כבר נסגרה.";
    public const string RoundNotActive = "אפשר לבצע את הפעולה הזו רק בסבב פעיל.";

    public const string AssignmentNotFound = "המשימה לא נמצאה בסבב הנוכחי.";
    public const string AssignmentTaken = "המשימה כבר נתפסה על ידי אח אחר.";
    public const string TaskNotSuitableForAge = "המשימה אינה מתאימה לגיל הילד.";
    public const string QuotaReached = "כבר נבחרו כל המשימות שמותר לבחור בסבב הזה.";
    public const string ChildNotApproved = "החשבון של הילד טרם אושר על ידי הורה.";
    public const string NotYourAssignment = "המשימה אינה משויכת לילד המחובר.";
    public const string AlreadyCompleted = "המשימה כבר סומנה כבוצעה בתקופה הנוכחית.";

    public const string SwapNotFound = "בקשת ההחלפה לא נמצאה.";
    public const string SwapOwnAssignmentRequired = "אפשר להחליף רק משימה שמשויכת לילד המבקש.";
    public const string SwapTargetInvalid = "המשימה המבוקשת חייבת להיות משויכת לאח אחר.";
    public const string SwapAlreadyPending = "כבר קיימת בקשת החלפה פתוחה על אחת מהמשימות.";
    public const string SwapNotYourDecision = "ההחלטה בשלב הזה שייכת לאח שאליו הופנתה הבקשה.";
    public const string SwapParentDecisionOnly = "ההחלטה בשלב הזה שייכת להורה.";
    public const string SwapAlreadyClosed = "בקשת ההחלפה כבר נסגרה.";
    public const string SwapAssignmentsChanged = "המשימות השתנו מאז הבקשה, ולכן אי אפשר לבצע את ההחלפה.";

    public const string SwapChatText = "בקשת החלפה: המשימה {0} תמורת המשימה {1}.";
}
