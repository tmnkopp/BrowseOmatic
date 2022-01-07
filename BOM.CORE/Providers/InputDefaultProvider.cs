using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BOM.CORE
{   public class InputDefaultSection {
        public InputDefaultSection()  {  }
        public InputDefaultSection(string ID)
        {
            this.ID = ID;
            this.InputDefaultItems = new List<InputDefaultItem>();
        }
        public string ID { get; set; } 
        public List<InputDefaultItem> InputDefaultItems { get; set; }
    }
    public class InputDefaultItem
    {
        public InputDefaultItem() { }
        public InputDefaultItem( string Pattern, string DefaultValue)
        {
            this.Pattern = Pattern;
            this.DefaultValue = DefaultValue; 
        } 
        public string Pattern { get; set; }
        public string DefaultValue { get; set; }
    }
    public class InputDefaultProvider : IAppSettingsProvider<InputDefaultSection>
    {
        #region CTOR 
        private readonly IConfiguration configuration; 
        private readonly ILogger logger;
        public InputDefaultProvider(
            IConfiguration configuration, 
            ILogger logger)
        {
            this.configuration = configuration; 
            this.logger = logger;
        }
        #endregion 
        #region PROPS
        public IEnumerable<InputDefaultSection> Items
        {
            get { return GetItems(); }
        }
        #endregion
        #region Methods 
        private IEnumerable<InputDefaultSection> GetItems()
        { 
            var sections = configuration.GetSection("InputDefault").GetChildren().AsEnumerable();
            
            List<InputDefaultSection> InputDefaults = new List<InputDefaultSection>();
            foreach (var item in sections)
            {
                logger.LogInformation("{o}", JsonConvert.SerializeObject(item));
                try
                {
                    InputDefaults.Add(new InputDefaultSection
                    {
                        ID=item.Key
                    }); 
                } 
                catch (Exception e)
                {
                    logger.LogError("{item}", JsonConvert.SerializeObject(item));
                    logger.LogError("{e}", e.Message);
                    Console.Write($" {e.Message} {e.StackTrace} "); 
                }  
            }  
            return InputDefaults.ToList();
        }
        #endregion
    }

}
 
