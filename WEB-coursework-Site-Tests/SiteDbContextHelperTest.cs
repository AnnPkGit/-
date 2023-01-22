using Moq;
using WEB_coursework_Site.DB.Context;
using WEB_coursework_Site.DB.Validators;

namespace WEB_coursework_Site_Tests
{
    internal class SiteDbContextHelperTest
    {
        private Mock<SiteDbcontext> _siteDbcontext;
        private Mock<EntityValidator> _entityValidator;

        [SetUp]
        public void Setup()
        {
            _siteDbcontext = new Mock<SiteDbcontext>(MockBehavior.Strict);
            _entityValidator = new Mock<EntityValidator>(MockBehavior.Strict);
        }

        [Test]
        public void ConstructorThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new SiteDbContextHelper(_siteDbcontext.Object, _entityValidator.Object, null));
        }
    }
}