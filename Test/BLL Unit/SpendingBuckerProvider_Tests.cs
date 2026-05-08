using AutoMapper;
using BudgetApp.BLL;
using BudgetApp.DAL;
using BudgetApp.Shared;
using Moq;
using System.Linq.Expressions;

namespace Test
{
    [TestFixture]
    [Category("BLL")]
    public class SpendingBucketProviderTests
    {
        private Mock<ISpendingBucketRepo> _mockRepo;
        private Mock<IMapper> _mockMapper;
        private SpendingBucketProvider _provider;

        private readonly List<SpendingBucket> _buckets = new()
        {
            new SpendingBucket { BucketId = 1, BucketLabel = "Groceries" },
            new SpendingBucket { BucketId = 2, BucketLabel = "Utilities" },
            new SpendingBucket { BucketId = 3, BucketLabel = "Entertainment" },
        };

        private readonly List<SpendingBucketDTO> _bucketDTOs = new()
        {
            new SpendingBucketDTO { BucketId = 1, BucketLabel = "Groceries" },
            new SpendingBucketDTO { BucketId = 2, BucketLabel = "Utilities" },
            new SpendingBucketDTO { BucketId = 3, BucketLabel = "Entertainment" },
        };

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<ISpendingBucketRepo>();
            _mockMapper = new Mock<IMapper>();
            _provider = new SpendingBucketProvider(_mockRepo.Object, _mockMapper.Object);

            // Default mapper behaviour: match by BucketId
            _mockMapper
                .Setup(m => m.Map<SpendingBucketDTO>(It.IsAny<SpendingBucket>()))
                .Returns((SpendingBucket src) =>
                    _bucketDTOs.First(d => d.BucketId == src.BucketId));
        }

        // GetAll

        [Test]
        public void GetAll_ReturnsAllBucketsAsDTOs()
        {
            _mockRepo.Setup(r => r.GetAll()).Returns(_buckets);

            var result = _provider.GetAll().ToList();

            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result.Select(r => r.BucketId),
                Is.EquivalentTo(new[] { 1, 2, 3 }));
        }

        [Test]
        public void GetAll_WhenRepoReturnsEmpty_ReturnsEmptyEnumerable()
        {
            _mockRepo.Setup(r => r.GetAll()).Returns(new List<SpendingBucket>());

            var result = _provider.GetAll();

            Assert.That(result, Is.Empty);
        }

        // GetByID

        [Test]
        public void GetByID_WithValidID_ReturnsMappedDTO()
        {
            _mockRepo
                .Setup(r => r.GetWhere(It.IsAny<Expression<Func<SpendingBucket, bool>>>()))
                .Returns((Expression<Func<SpendingBucket, bool>> predicate) =>
                    _buckets.Where(predicate.Compile()));

            var result = _provider.GetByID(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.BucketId, Is.EqualTo(1));
            Assert.That(result.BucketLabel, Is.EqualTo("Groceries"));
        }

        [Test]
        public void GetByID_WithInvalidID_ReturnsNull()
        {
            _mockRepo
                .Setup(r => r.GetWhere(It.IsAny<Expression<Func<SpendingBucket, bool>>>()))
                .Returns(Enumerable.Empty<SpendingBucket>());

            var result = _provider.GetByID(999);

            Assert.That(result, Is.Null);
        }

        // GetByIDs

        [Test]
        public void GetByIDs_WithValidIDs_ReturnsMappedDTOs()
        {
            _mockRepo
                .Setup(r => r.GetWhere(It.IsAny<Expression<Func<SpendingBucket, bool>>>()))
                .Returns((Expression<Func<SpendingBucket, bool>> predicate) =>
                    _buckets.Where(predicate.Compile()));

            var result = _provider.GetByIDs(new[] { 1, 3 }).ToList();

            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.Select(r => r.BucketId),
                Is.EquivalentTo(new[] { 1, 3 }));
        }

        [Test]
        public void GetByIDs_WithNoMatchingIDs_ReturnsEmptyEnumerable()
        {
            _mockRepo
                .Setup(r => r.GetWhere(It.IsAny<Expression<Func<SpendingBucket, bool>>>()))
                .Returns(Enumerable.Empty<SpendingBucket>());

            var result = _provider.GetByIDs(new[] { 99, 100 });

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetByIDs_WithEmptyArray_ReturnsEmptyEnumerable()
        {
            _mockRepo
                .Setup(r => r.GetWhere(It.IsAny<Expression<Func<SpendingBucket, bool>>>()))
                .Returns(Enumerable.Empty<SpendingBucket>());

            var result = _provider.GetByIDs(Array.Empty<int>());

            Assert.That(result, Is.Empty);
        }
    }
}