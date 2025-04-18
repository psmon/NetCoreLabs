﻿using ActorLib.Persistent;
using ActorLib.Persistent.Model;
using Raven.Client.Documents;
using Raven.Client.Documents.Queries.Vector;
using Xunit.Abstractions;

namespace ActorLibTest.Persistent;

public class TravelReviewRepositoryTests : TestKitXunit
{
    private readonly IDocumentStore _store;
    private readonly TravelReviewRepository _repository;

    public TravelReviewRepositoryTests(ITestOutputHelper output) : base(output)
    {
        // RavenDB In-Memory Document Store 설정
        _store = new DocumentStore
        {
            Urls = new[] { "http://127.0.0.1:9000" },
            Database = "net-core-labs"
        };
        _store.Initialize();
        
        new TravelReviewIndex().Execute(_store);

        _repository = new TravelReviewRepository(_store);

        // 샘플 데이터 추가
        AddSampleData();
    }

    private void AddSampleData()
    {
        var random = new Random();
        var categories = new[] { "Nature", "City", "Adventure", "Relaxation" };

        using (var session = _store.OpenSession())
        {
            for (int i = 0; i < 50; i++)
            {
                var review = new TravelReview
                {
                    Title = $"Review {i + 1}",
                    Content = $"This is a sample review content {i + 1}",
                    Category = categories[random.Next(categories.Length)],
                    Latitude = random.NextDouble() * 180 - 90, // -90 to 90
                    Longitude = random.NextDouble() * 360 - 180, // -180 to 180
                    CreatedAt = DateTime.UtcNow,
                    TagsEmbeddedAsSingle = new RavenVector<float>(new float[] { random.Next(1, 10), random.Next(1, 10) }),
                    TagsEmbeddedAsBase64 = new List<string> { "zczMPc3MTD6amZk+", "mpmZPs3MzD4AAAA/" },
                    TagsEmbeddedAsInt8 = new sbyte[][]
                    {
                        VectorQuantizer.ToInt8(new float[] { 0.1f, 0.2f }),
                        VectorQuantizer.ToInt8(new float[] { 0.3f, 0.4f })
                    }
                };
                session.Store(review);
            }
            
            // 테스트용 벡터와 정확히 일치하는 데이터 추가
            session.Store(new TravelReview
            {
                Title = "Vector Test Review",
                Content = "This is a vector test review",
                Category = "Test",
                Latitude = 37.7749,
                Longitude = -122.4194,
                CreatedAt = DateTime.UtcNow,
                TagsEmbeddedAsSingle = new RavenVector<float>(new float[] { 6.6f, 7.7f }),
                TagsEmbeddedAsBase64 = new List<string> { "zczMPc3MTD6amZk+", "mpmZPs3MzD4AAAA/" },
                TagsEmbeddedAsInt8 = new sbyte[][]
                {
                    VectorQuantizer.ToInt8(new float[] { 0.1f, 0.2f }),
                    VectorQuantizer.ToInt8(new float[] { 0.3f, 0.4f })
                }
            });
            
            session.SaveChanges();
        }
    }

    [Fact]
    public void TestAddReview()
    {
        var review = new TravelReview
        {
            Title = "New Review",
            Content = "This is a new review",
            Category = "Nature",
            Latitude = 37.7749,
            Longitude = -122.4194,
            CreatedAt = DateTime.UtcNow
        };

        _repository.AddReview(review);

        using (var session = _store.OpenSession())
        {
            var result = session.Query<TravelReview>().FirstOrDefault(r => r.Title == "New Review");
            Assert.NotNull(result);
            Assert.Equal("Nature", result.Category);
        }
    }

    [Fact]
    public void TestSearchReviews()
    {
        var results = _repository.SearchReviews("sample", 0, 0, 10000, "Nature");
        Assert.NotEmpty(results);
        Assert.All(results, r => Assert.Contains("sample", r.Content));
        
        // Output results
        foreach (var result in results)
        {
            output.WriteLine($"Title: {result.Title}, Latitude: {result.Latitude}, Longitude: {result.Longitude}");
        }
    }
    
    [Fact]
    public void TestSearchReviewsByRadius()
    {
        // Arrange
        double latitude = 37.7749; // 샌프란시스코
        double longitude = -122.4194;
        double radiusKm = 50; // 50km 반경

        // Act
        var results = _repository.SearchReviewsByRadius(latitude, longitude, radiusKm);
        
        // Assert
        Assert.NotEmpty(results);
        
        // Output results
        foreach (var result in results)
        {
            output.WriteLine($"Title: {result.Title}, Latitude: {result.Latitude}, Longitude: {result.Longitude}");
        }
        
    }

    [Fact]
    public void TestSearchReviewsByVector()
    {
        var queryVector = new float[] { 6.599999904632568f, 7.699999809265137f };
        var results = _repository.SearchReviewsByVector(queryVector, 5);
        Assert.NotEmpty(results);
        
        // Output results
        foreach (var result in results)
        {
            output.WriteLine($"Title: {result.Title}, Latitude: {result.Latitude}, Longitude: {result.Longitude}");
        }
    }
}