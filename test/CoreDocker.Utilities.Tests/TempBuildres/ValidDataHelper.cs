using CoreDocker.Dal.Models;
using CoreDocker.Dal.Models.Projects;
using CoreDocker.Dal.Models.Users;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Generators;

namespace CoreDocker.Utilities.Tests.TempBuildres
{
  public static class ValidDataHelper
  {
    public static ISingleObjectBuilder<T> WithValidData<T>(this ISingleObjectBuilder<T> value)
    {
      return value.With(ValidData);
    }

    public static IListBuilder<T> WithValidData<T>(this IListBuilder<T> value)
    {
      return value.All().With(ValidData);
    }

    #region Private Methods

    private static T ValidData<T>(T value)
    {
      var project = value as Project;
      if (project != null)
        project.Name = GetRandom.String(20);

      var user = value as User;
      if (user != null)
      {
        user.Name = GetRandom.String(20);
        user.Email = (GetRandom.String(20) + "@nomailmail.com").ToLower();
        user.HashedPassword = GetRandom.String(20);
        user.Roles.Add("Guest");
      }
      return value;
    }

    #endregion
  }
}