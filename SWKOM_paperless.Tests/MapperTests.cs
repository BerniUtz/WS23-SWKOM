using NUnit.Framework;
using SWKOM_paperless.BusinessLogic.Mapper;

namespace Paperless.Tests
{
    [TestFixture]
    public class BaseMapperTests
    {
        private BaseMapper<SampleEntity, SampleDto> _mapper;

        [SetUp]
        public void SetUp()
        {
            _mapper = new BaseMapper<SampleEntity, SampleDto>();
        }

        [Test]
        public void DtoToEntity_MapsDtoToEntityCorrectly()
        {
            // Arrange
            var dto = new SampleDto { Id = 1, Name = "Test" };

            // Act
            var entity = _mapper.DtoToEntity(dto);

            // Assert
            Assert.NotNull(entity);
            Assert.AreEqual(dto.Id, entity.Id);
            Assert.AreEqual(dto.Name, entity.Name);
        }

        [Test]
        public void EntityToDto_MapsEntityToDtoCorrectly()
        {
            // Arrange
            var entity = new SampleEntity { Id = 1, Name = "Test" };

            // Act
            var dto = _mapper.EntityToDto(entity);

            // Assert
            Assert.NotNull(dto);
            Assert.AreEqual(entity.Id, dto.Id);
            Assert.AreEqual(entity.Name, dto.Name);
        }

        [Test]
        public void Map_WithNonNullJsonNullable_ReturnsValue()
        {
            // Arrange
            var jsonNullable = JsonNullable<string>.Of("Test");

            // Act
            var result = _mapper.Map(jsonNullable);

            // Assert
            Assert.AreEqual("Test", result);
        }

        [Test]
        public void Map_WithNullJsonNullable_ReturnsDefault()
        {
            // Arrange
            JsonNullable<string> jsonNullable = null;

            // Act
            var result = _mapper.Map(jsonNullable);

            // Assert
            Assert.Null(result);
        }

        [Test]
        public void Map_WithNonNullValue_ReturnsJsonNullable()
        {
            // Arrange
            var value = "Test";

            // Act
            var result = _mapper.Map(value);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(value, result.Value);
        }

        [Test]
        public void Map_WithNullValue_ReturnsNullJsonNullable()
        {
            // Arrange
            string value = null;

            // Act
            var result = _mapper.Map(value);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsPresent);
        }
    }

    public class SampleEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class SampleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
