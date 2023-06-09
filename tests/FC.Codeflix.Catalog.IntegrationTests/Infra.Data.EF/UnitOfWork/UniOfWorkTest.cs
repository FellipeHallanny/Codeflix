using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using UnitOfWorkInfra = FC.Codeflix.Catalog.Infra.Data.EF;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.UnitOfWork;

[Collection(nameof(UniOfWorkTestFixture))]
public class UniOfWorkTest
{
    private readonly UniOfWorkTestFixture _fixture;
    
    public UniOfWorkTest(UniOfWorkTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(Commit))]
    [Trait("Integration/Infra.Data", "UnitOfWork - Persistence")]
    public async Task Commit()
    {
        var dbContext = _fixture.CreateDbContext();
        var exempleCategoriesList = _fixture.GetExampleCategoriesList();
        await dbContext.Categories.AddRangeAsync(exempleCategoriesList);
        var unitOfWork = new UnitOfWorkInfra.UnitOfWork(dbContext);
        
        await unitOfWork.Commit(CancellationToken.None);
        
        var assertDbContext = _fixture.CreateDbContext(true);
        var savedCategories = assertDbContext.Categories.AsNoTracking().ToList();
        savedCategories.Should().HaveCount(exempleCategoriesList.Count);
    }
    
    [Fact(DisplayName = nameof(Rollback))]
    [Trait("Integration/Infra.Data", "UnitOfWork - Persistence")]
    public async Task Rollback()
    {
        var dbContext = _fixture.CreateDbContext();
        var unitOfWork = new UnitOfWorkInfra.UnitOfWork(dbContext);
        
        var task = async () => await unitOfWork.Rollback(CancellationToken.None);
        
        await task.Should().NotThrowAsync();
    }
}