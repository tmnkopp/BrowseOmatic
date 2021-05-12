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
{   public class InputDefault {
        public InputDefault()  {  }
        public InputDefault(string ID)
        {
            this.ID = ID;
        }
        public string ID { get; set; } 
    }
    public class InputDefaultItem
    {
        public InputDefaultItem() { }
        public InputDefaultItem(InputDefault InputDefault,string Pattern, string DefaultValue)
        {
            this.Pattern = Pattern;
            this.DefaultValue = DefaultValue;
            this.InputDefault = InputDefault;
        }
        public InputDefault InputDefault { get; set;}
        public string Pattern { get; set; }
        public string DefaultValue { get; set; }
    }
    public class InputDefaultProvider : IAppSettingProvider<InputDefault>
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
        public IEnumerable<InputDefault> Items
        {
            get { return GetItems(); }
        }
        #endregion
        #region Methods 
        private IEnumerable<InputDefault> GetItems()
        { 
            var sections = configuration.GetSection("InputDefault").GetChildren().AsEnumerable();
            
            List<InputDefault> InputDefaults = new List<InputDefault>();
            foreach (var item in sections)
            {
                logger.LogInformation("{o}", JsonConvert.SerializeObject(item));
                try
                {
                    InputDefaults.Add(new InputDefault
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
 
