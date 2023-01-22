using WEB_coursework_Site.DB.Validators;
using WEB_coursework_Site.Models;

namespace WEB_coursework_Site_Tests
{
    public class EntityValidatorTests
    {
        [Test]
        public void CreateAndValidateUser_ReturnsSuccessfulResult()
        {
            //Arrange
            var userModel = new UserModel()
            {
                SecretQuestionAnswear = "red",
                Email = "email@email.com",
                Login = "login",
                Password = "password",
                RealName = "realName",
            };
            var entityValidator = new EntityValidator();

            //Act
            var result = entityValidator.CreateAndValidateUser(userModel);

            //Asser
            Assert.IsTrue(result.IsSuccessful);
            Assert.IsTrue(result.Value.Email.Equals(userModel.Email));
            Assert.IsTrue(result.Value.SecretQuestionAnswear.Equals(userModel.SecretQuestionAnswear));
            Assert.IsTrue(result.Value.RealName.Equals(userModel.RealName));
            Assert.IsTrue(result.Value.Password.Equals(userModel.Password));
            Assert.IsTrue(result.Value.Login.Equals(userModel.Login));
        }

        [Test]
        public void CreateAndValidateUser_ReturnsFailedResult_InvalidSecretQuestionAnswear()
        {
            //Arrange
            var userModel = new UserModel()
            {
                SecretQuestionAnswear = String.Empty,
                Email = "email@email.com",
                Login = "login",
                Password = "password",
                RealName = "realName",
            };
            var entityValidator = new EntityValidator();

            //Act
            var result = entityValidator.CreateAndValidateUser(userModel);

            //Asser
            Assert.IsTrue(!result.IsSuccessful);
        }

        [Test]
        public void CreateAndValidateUser_ReturnsFailedResult_InvalidEmail()
        {
            //Arrange
            var userModel = new UserModel()
            {
                SecretQuestionAnswear = "red",
                Email = String.Empty,
                Login = "login",
                Password = "password",
                RealName = "realName",
            };
            var entityValidator = new EntityValidator();

            //Act
            var result = entityValidator.CreateAndValidateUser(userModel);

            //Asser
            Assert.IsTrue(!result.IsSuccessful);
        }

        [Test]
        public void CreateAndValidateUser_ReturnsFailedResult_InvalidLogin()
        {
            //Arrange
            var userModel = new UserModel()
            {
                SecretQuestionAnswear = "red",
                Email = "email@email.com",
                Login = String.Empty,
                Password = "password",
                RealName = "realName",
            };
            var entityValidator = new EntityValidator();

            //Act
            var result = entityValidator.CreateAndValidateUser(userModel);

            //Asser
            Assert.IsTrue(!result.IsSuccessful);
        }

        [Test]
        public void CreateAndValidateUser_ReturnsFailedResult_InvalidPassword()
        {
            //Arrange
            var userModel = new UserModel()
            {
                SecretQuestionAnswear = "red",
                Email = "email@email.com",
                Login = "login",
                Password = String.Empty,
                RealName = "realName",
            };
            var entityValidator = new EntityValidator();

            //Act
            var result = entityValidator.CreateAndValidateUser(userModel);

            //Asser
            Assert.IsTrue(!result.IsSuccessful);
        }

        [Test]
        public void CreateAndValidateUser_ReturnsFailedResult_InvalidRealName()
        {
            //Arrange
            var userModel = new UserModel()
            {
                SecretQuestionAnswear = "red",
                Email = "email@email.com",
                Login = "login",
                Password = "password",
                RealName = String.Empty,
            };
            var entityValidator = new EntityValidator();

            //Act
            var result = entityValidator.CreateAndValidateUser(userModel);

            //Asser
            Assert.IsTrue(!result.IsSuccessful);
        }
    }
}