using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace BOM.CORE
{
    public class ProfileProvider : IAppSettingProvider<Profile>
    {
        #region CTOR 
        private readonly IConfiguration configuration;
        public ProfileProvider(
            IConfiguration Configuration)
        {
            configuration = Configuration;
        }
        #endregion 
        #region PROPS
        public IEnumerable<Profile> Items
        {
            get { return GetItems(); }
        }
        #endregion
        #region Methods
        public IEnumerable<Profile> Get(Expression<Func<Profile, bool>> predicate)
        {
            return Items.AsQueryable().Where(predicate).AsEnumerable<Profile>();
        }
        private IEnumerable<Profile> GetItems()
        {
            var section = configuration.GetSection("profiles"); 
            return section.GetChildren()
                 .Select(
                    s => new Profile
                    {
                        Name = s["profile"].Split(";")[0],
                        UserName = s["profile"].Split(";")[1],
                        Password = s["profile"].Split(";")[2],
                    }
                 ).ToList();
        }
        #endregion
    }

}
