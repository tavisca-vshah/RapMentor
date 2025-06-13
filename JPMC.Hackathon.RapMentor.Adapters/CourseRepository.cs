using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using JPMC.Hackathon.RapMentor.Adapter.Dynamodb.Model;
using JPMC.Hackathon.RapMentor.Contract.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JPMC.Hackathon.RapMentor.Adapter.Dynamodb
{
    public class CourseRepository : ICourseRepository
    {
        private readonly IDynamoDbContextProvider _dynamoDbContextProvider;

        public IDynamoDbFactory _dynamoDbClientFactory { get; }

        public CourseRepository(IDynamoDbFactory dynamoDbFactory, IDynamoDbContextProvider dynamoDbContextProvider)
        {
            _dynamoDbClientFactory = dynamoDbFactory;
            _dynamoDbContextProvider = dynamoDbContextProvider;
        }

        public async Task<Contract.Models.Course> CreateCourseAsync(Contract.Models.Course course)
        {
            var dbSetting = await GetDynamoDbSetting();
            var client = await _dynamoDbClientFactory.GetClientAsync(dbSetting);

            using (var context = _dynamoDbContextProvider.GetDynamoDbContext(client))
            {
                var dynamoDBOperationConfig = new BatchWriteConfig
                {
                    OverrideTableName = dbSetting.CourseTableName
                };

                var saveObject = course.ToCourseDBObject();
                if (string.IsNullOrWhiteSpace(saveObject.Id))
                {
                    saveObject.Id = Guid.NewGuid().ToString();
                }

                var courseBatch = context.CreateBatchWrite<CourseDataobject>(dynamoDBOperationConfig);
                courseBatch.AddPutItem(saveObject);
                await courseBatch.ExecuteAsync();

                dynamoDBOperationConfig = new BatchWriteConfig
                {
                    OverrideTableName = dbSetting.ModuleTableName
                };

                var moduleBatch = context.CreateBatchWrite<ModuleDataObject>(dynamoDBOperationConfig);

                var moduleSaveData = course.Modules.Where(x => IsModuleValid(x)).Select(x => x.ToModuleObject(saveObject.Id)).ToList();
                moduleSaveData.ForEach(x => { x.ModuleId = Guid.NewGuid().ToString(); moduleBatch.AddPutItem(x); });
                await moduleBatch.ExecuteAsync();
                return saveObject.ToCourseModel();
            }
        }

        private static bool IsModuleValid(Contract.Models.Module x)
        {
            return !string.IsNullOrEmpty(x.Content)
                && !string.IsNullOrEmpty(x.Title);
        }

        private async Task<DynamoDbSettings> GetDynamoDbSetting()
        {
            return new DynamoDbSettings
            {
                CourseTableName = "Courses",
                ModuleTableName = "modules",
                Region = "us-east-1"
            };
        }

        public async Task PublishCourseAsync(string id, Course course)
        {
            var dbSetting = await GetDynamoDbSetting();
            var client = await _dynamoDbClientFactory.GetClientAsync(dbSetting);

            using (var context = _dynamoDbContextProvider.GetDynamoDbContext(client))
            {
                var dynamoDBOperationConfig = new BatchWriteConfig
                {
                    OverrideTableName = dbSetting.CourseTableName
                };

                var saveObject = course.ToCourseDBObject();
                saveObject.Id = id;
                saveObject.CourseStatus = CourseStatus.Published.ToString();
                var courseBatch = context.CreateBatchWrite<CourseDataobject>(dynamoDBOperationConfig);
                courseBatch.AddPutItem(saveObject);
                await courseBatch.ExecuteAsync();
            }
        }

        public async Task<Contract.Models.Course> GetAsync(string id)
        {
            var dynamoDbSettings = await GetDynamoDbSetting();

            var client = await _dynamoDbClientFactory.GetClientAsync(dynamoDbSettings);
            try
            {
                using (var context = _dynamoDbContextProvider.GetDynamoDbContext(client))
                {
                    var queryConfig = new QueryConfig
                    {
                        OverrideTableName = dynamoDbSettings.CourseTableName
                    };

                    var courseDataObject = await context.LoadAsync<CourseDataobject>(id, queryConfig);
                    if (courseDataObject != null)
                    {
                        var moduleDataObject = await GetModuleAsync(id);
                        var course = courseDataObject.ToCourseModel();
                        moduleDataObject.ForEach(x => course.Modules.Add(x.ToModuleModel()));
                        return course;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (AmazonDynamoDBException ex)
            {
                throw new Exception("Something went wrong try again later");
            }
        }

        public async Task<List<Contract.Models.Course>> GetAllAsync()
        {
            var dynamoDbSettings = await GetDynamoDbSetting();

            var client = await _dynamoDbClientFactory.GetClientAsync(dynamoDbSettings);
            try
            {
                var courseDataObject = new List<CourseDataobject>();
                using (var context = _dynamoDbContextProvider.GetDynamoDbContext(client))
                {
                    var dynamoDBOperationConfig = new ScanConfig
                    {
                        OverrideTableName = dynamoDbSettings.CourseTableName
                    };

                    var search = context.ScanAsync<CourseDataobject>(new List<ScanCondition>(), dynamoDBOperationConfig);

                    do
                    {
                        List<CourseDataobject> responseItems = await search.GetNextSetAsync();
                        courseDataObject.AddRange(responseItems);
                    } while (!search.IsDone);

                    var courses = courseDataObject.Select(x => x.ToCourseModel()).ToList();
                    foreach (var course in courses)
                    {
                        var moduleDataObject = await GetModuleAsync(course.Id);
                        course.Modules.AddRange(moduleDataObject.Select(x => x.ToModuleModel()).ToList());
                    }

                    return courses;
                }
            }
            catch (AmazonDynamoDBException ex)
            {
                throw new Exception("Something went wrong try again later");
            }
        }

        public async Task<Contract.Models.Course> UpdateCourseAsync(Contract.Models.Course course)
        {
            var dbSetting = await GetDynamoDbSetting();
            var client = await _dynamoDbClientFactory.GetClientAsync(dbSetting);

            using (var context = _dynamoDbContextProvider.GetDynamoDbContext(client))
            {
                var dynamoDBOperationConfig = new BatchWriteConfig
                {
                    OverrideTableName = dbSetting.CourseTableName
                };

                var saveObject = course.ToCourseDBObject();
                if (string.IsNullOrWhiteSpace(saveObject.Id))
                {
                    saveObject.Id = Guid.NewGuid().ToString();
                }

                var courseBatch = context.CreateBatchWrite<CourseDataobject>(dynamoDBOperationConfig);
                courseBatch.AddPutItem(saveObject);
                await courseBatch.ExecuteAsync();

                dynamoDBOperationConfig = new BatchWriteConfig
                {
                    OverrideTableName = dbSetting.ModuleTableName
                };

                var moduleBatch = context.CreateBatchWrite<ModuleDataObject>(dynamoDBOperationConfig);

                var moduleSaveData = course.Modules.Where(x => IsModuleValid(x)).Select(x => x.ToModuleObject(saveObject.Id)).ToList();
                foreach (var module in moduleSaveData)
                {
                    if (string.IsNullOrEmpty(module.ModuleId))
                    {
                        module.ModuleId = Guid.NewGuid().ToString();
                    }
                    moduleBatch.AddPutItem(module);
                }

                await moduleBatch.ExecuteAsync();
            }
            return course;
        }

        private async Task<List<ModuleDataObject>> GetModuleAsync(string courseId)
        {
            var dynamoDbSettings = await GetDynamoDbSetting();

            var client = await _dynamoDbClientFactory.GetClientAsync(dynamoDbSettings);
            try
            {
                var moduleDataObject = new List<ModuleDataObject>();
                using (var context = _dynamoDbContextProvider.GetDynamoDbContext(client))
                {
                    var queryConfig = new QueryConfig
                    {
                        OverrideTableName = dynamoDbSettings.ModuleTableName
                    };

                    var search = context.QueryAsync<ModuleDataObject>(courseId, queryConfig);

                    do
                    {
                        List<ModuleDataObject> responseItems = await search.GetNextSetAsync();
                        moduleDataObject.AddRange(responseItems);
                    } while (!search.IsDone);

                    return moduleDataObject;
                }
            }
            catch (AmazonDynamoDBException ex)
            {
                throw new Exception("Something went wrong try again later");
            }
        }

        public async Task DeleteModuleAsync(string courseid, List<string> modules)
        {
            var dynamoDbSettings = await GetDynamoDbSetting();

            var client = await _dynamoDbClientFactory.GetClientAsync(dynamoDbSettings);
            try
            {
                using (var context = _dynamoDbContextProvider.GetDynamoDbContext(client))
                {
                    var queryConfig = new BatchWriteConfig
                    {
                        OverrideTableName = dynamoDbSettings.ModuleTableName
                    };

                    var moduleUpdateBatch = context.CreateBatchWrite<ModuleDataObject>(queryConfig);
                    modules.ForEach(moduleId => moduleUpdateBatch.AddDeleteKey(courseid, moduleId));

                    await moduleUpdateBatch.ExecuteAsync();
                }
            }
            catch (AmazonDynamoDBException ex)
            {
                throw new Exception("Something went wrong try again later");
            }
        }

        public async Task DeleteAsync(string courseId)
        {
            var dynamoDbSettings = await GetDynamoDbSetting();

            var client = await _dynamoDbClientFactory.GetClientAsync(dynamoDbSettings);
            try
            {
                using (var context = _dynamoDbContextProvider.GetDynamoDbContext(client))
                {
                    var queryConfig = new BatchWriteConfig
                    {
                        OverrideTableName = dynamoDbSettings.CourseTableName
                    };

                    var moduleUpdateBatch = context.CreateBatchWrite<CourseDataobject>(queryConfig);
                    moduleUpdateBatch.AddDeleteKey(courseId);

                    await moduleUpdateBatch.ExecuteAsync();
                }
            }
            catch (AmazonDynamoDBException ex)
            {
                throw new Exception("Something went wrong try again later");
            }
        }
    }
}
