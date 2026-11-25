using System.Net;
using System.Net.Http.Json;
using Hpuni.Application;
using Xunit;

namespace Hpuni.IntegrationTests;

[Collection("SQL Server")]
public class KhtcWorkflowTests(FinanceFactory f):IClassFixture<FinanceFactory>
{
    static Task<HttpResponseMessage> Post(HttpClient c,string path,object data) {
        var request=new HttpRequestMessage(HttpMethod.Post,"/api/v1/"+path){Content=JsonContent.Create(data)};
        request.Headers.Add("Idempotency-Key",Guid.NewGuid().ToString());return c.SendAsync(request);
    }
    static async Task<ProposalView> Ok(Task<HttpResponseMessage> action) {
        var r=await action;Assert.True(r.IsSuccessStatusCode,await r.Content.ReadAsStringAsync());return (await r.Content.ReadFromJsonAsync<ProposalView>())!;
    }
    async Task<HttpClient> User(string role) {
        var a=await f.Admin();var u=await SqlFactory.Create(a,role,"it");
        Assert.Equal(HttpStatusCode.OK,(await a.PutAsJsonAsync($"/api/v1/users/{u.Id}/scope",new ConfigureScope("all",[],[],u.Version))).StatusCode);
        return await f.Login(u.UserName);
    }
    [Fact]
    public async Task AppraisalRequiredAndReturnMustBeReappraisedBeforeAllocation() {
        using var a=await User("accounting");using var k=await User("khtc");using var e=await User("executive");
        var p=await Ok(Post(a,"finance/proposals",new SaveProposal("it",2049,"general","Dự toán kiểm thử KHTC",null,[new("Thiết bị",1,100)])));
        Task<HttpResponseMessage> Act(HttpClient c,string op,string? note=null)=>Post(c,$"finance/proposals/{p.Id}/{op}",new ProposalCommand(p.Version,note));
        p=await Ok(Act(a,"submit"));Assert.Equal("reviewing",p.Status);
        Assert.Equal(HttpStatusCode.BadRequest,(await Act(e,"approve","Đồng ý")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden,(await Act(a,"appraise","Đạt")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest,(await Act(k,"appraise","")).StatusCode);
        p=await Ok(Act(k,"return-review","Bổ sung báo giá"));Assert.Equal("returned",p.Status);
        var edit=new SaveProposal("it",2049,"general","Dự toán đã bổ sung",null,[new("Thiết bị",1,90)],p.Version);
        var firstEdit=new HttpRequestMessage(HttpMethod.Put,"/api/v1/finance/proposals/"+p.Id){Content=JsonContent.Create(edit)};firstEdit.Headers.Add("Idempotency-Key",Guid.NewGuid().ToString());
        p=await Ok(a.SendAsync(firstEdit));
        p=await Ok(Act(a,"submit"));p=await Ok(Act(k,"appraise","Đủ hồ sơ"));Assert.Equal("pending",p.Status);
        Assert.Equal(HttpStatusCode.Forbidden,(await Act(k,"approve","Đồng ý")).StatusCode);
        var detail=await e.GetFromJsonAsync<ProposalView>("/api/v1/finance/proposals/"+p.Id);Assert.Equal("Đủ hồ sơ",detail!.AppraisalNote);
        p=await Ok(Act(e,"return","Làm rõ nội dung"));Assert.Equal("returned",p.Status);
        var request=new HttpRequestMessage(HttpMethod.Put,"/api/v1/finance/proposals/"+p.Id){Content=JsonContent.Create(edit with{Version=p.Version})};request.Headers.Add("Idempotency-Key",Guid.NewGuid().ToString());
        p=await Ok(a.SendAsync(request));p=await Ok(Act(a,"submit"));p=await Ok(Act(k,"appraise","Đã kiểm tra lại"));p=await Ok(Act(e,"approve","Phê duyệt"));
        Assert.Equal("approved",p.Status);
        var balance=await a.GetFromJsonAsync<BudgetBalance>("/api/v1/finance/balance?departmentId=it&year=2049");Assert.Equal(90,balance!.Allocated);
        Assert.Equal(HttpStatusCode.BadRequest,(await Act(e,"approve","Lặp")).StatusCode);
    }
}
