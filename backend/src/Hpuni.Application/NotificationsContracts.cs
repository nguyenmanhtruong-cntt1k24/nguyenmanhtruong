namespace Hpuni.Application;

public record NotificationView(string Id,string Kind,string SourceId,string Title,string Message,
    DateTimeOffset At,string Severity,string Url,bool Read);
public record NotificationPage(NotificationView[] Items,int Total,int Unread,int Page,int PageSize,DateTimeOffset GeneratedAt);
