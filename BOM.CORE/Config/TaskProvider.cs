using Microsoft.Extensions.Configuration; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace BOM.CORE
{

    public class TaskProvider: IAppSettingProvider<Task>
    {
        #region CTOR
        private readonly IConfiguration configuration;
        public TaskProvider(
            IConfiguration Configuration)
        {
            configuration = Configuration;
        }
        #endregion 
        #region PROPS
        public IEnumerable<Task> Items
        {
            get { return GetItems(); }
        } 
        #endregion 
        #region Methods
        public IEnumerable<Task> Get(Expression<Func<Task, bool>> predicate)
        {
            return Items.AsQueryable().Where(predicate).AsEnumerable<Task>();
        }
        private IEnumerable<Task> GetItems()
        { 
            var tasksection = configuration.GetSection("tasks");

            return tasksection.GetChildren()
                 .Select(
                    cs => new Task
                    {
                        Name = cs["name"],  
                        TaskSteps = cs.GetSection("steps").GetChildren().Select(
                            ts => new TaskStep()
                            {
                                cmd = ts["cmd"],
                                args = ts.GetSection("args").GetChildren().Select(s => s.Value).ToArray()
                            }
                            ).ToList()
                    }
                 ).ToList();
        }
        #endregion
    }
}
