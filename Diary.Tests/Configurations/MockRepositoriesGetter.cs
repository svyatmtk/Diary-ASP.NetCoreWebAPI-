using AutoMapper;
using Diary.Domain.Entity;
using Diary.Domain.Interfaces.Repositories;
using MockQueryable.Moq;
using Moq;

namespace Diary.Tests.Configurations;

public static class MockRepositoriesGetter
{
    public static Mock<IBaseRepository<Report>> GetMockReportRepository()
    {
        var mock = new Mock<IBaseRepository<Report>>();
        var testData = GetReportsMockData().BuildMockDbSet();

        mock.Setup(x => x.GetAll()).Returns(() => testData.Object);
        return mock;
    }

    public static Mock<IBaseRepository<User>> GetMockUserRepository()
    {
        var mock = new Mock<IBaseRepository<User>>();
        var testData = GetUsersMockData().BuildMockDbSet();

        mock.Setup(x => x.GetAll()).Returns(() => testData.Object);
        return mock;
    }

    public static IQueryable<Report> GetReportsMockData()
    {
        return new List<Report>()
        {
            new Report()
            {
                Id = 1,
                Name = "Test Report 1",
                Description = "Test Rep 1 Descript",
                CreatedAt = DateTime.Now.AddDays(-2),
                UpdatedAt = DateTime.Now.AddDays(-2)
            },
            new Report()
            {
                Id = 2,
                Name = "Test Report 2",
                Description = "Test Rep 2 Descript",
                CreatedAt = DateTime.Now.AddDays(-1),
                UpdatedAt = DateTime.Now.AddDays(-1)
            }
        }.AsQueryable();
    }

    public static IQueryable<User> GetUsersMockData()
    {
        return new List<User>()
        {
            new User()
            {
                Id = 1,
                Login = "svmtk",
                Password = "111",
                CreatedAt = DateTime.Now.AddDays(-3),
                UpdatedAt = DateTime.Now.AddDays(-3),
            },
            new User()
            {
                Id = 2,
                Login = "svmtk22",
                Password = "222",
                CreatedAt = DateTime.Now.AddDays(-3),
                UpdatedAt = DateTime.Now.AddDays(-3),
            }
        }.AsQueryable();
    }
}