using Diary.Application.Services;
using Diary.Tests.Configurations;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Xunit;


namespace Diary.Tests;

public class ReportServiceTests
{
    [Fact]
    public async System.Threading.Tasks.Task GetReportById_ShouldBe_NotNull()
    {
        //Arrange
        var mockReportRepository = MockRepositoriesGetter.GetMockReportRepository();
        var mockDistributedCache = new Mock<IDistributedCache>();
        var reportService = new ReportService(mockReportRepository.Object, null, null, null, null, null, null,
            mockDistributedCache.Object, null);
        
        //Act
        var result = await reportService.GetReportByIdAsync(1);
        
        //Assert
        Assert.NotNull(result);
    }
    
    
}