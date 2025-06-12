using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using JPMC.Hackathon.RapMentor.Adapter.Dynamodb.Model;
using JPMC.Hackathon.RapMentor.Contract.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JPMC.Hackathon.RapMentor.Adapter.Dynamodb
{
    public class CourseRepository : ICourseRepository
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IDynamoDbContextProvider _dynamoDbContextProvider;

        public IDynamoDbFactory _dynamoDbClientFactory { get; }

        public CourseRepository(IDynamoDbFactory dynamoDbFactory, IConfigurationProvider configurationProvider, IDynamoDbContextProvider dynamoDbContextProvider)
        {
            _dynamoDbClientFactory = dynamoDbFactory;
            _configurationProvider = configurationProvider;
            _dynamoDbContextProvider = dynamoDbContextProvider;
        }

        public async Task<Contract.Models.Course> CreateCourseAsync(Contract.Models.Course course)
        {
            var dbSetting = await GetDynamoDbSetting();
            var client = await _dynamoDbClientFactory.GetClientAsync(dbSetting);
            var saveObject = course.ToCourseDBObject();

            using (var context = _dynamoDbContextProvider.GetDynamoDbContext(client))
            {
                var dynamoDBOperationConfig = new DynamoDBOperationConfig
                {
                    OverrideTableName = dbSetting.TableName
                };

                var configurationBatch = context.CreateBatchWrite<CourseDataobject>(dynamoDBOperationConfig);
                configurationBatch.AddPutItem(saveObject);
            }
            return course;
        }

        private async Task<DynamoDbSettings> GetDynamoDbSetting()
        {
            var setting = await _configurationProvider.GetConfiguration("dynamodb");
            var dbSetting = System.Text.Json.JsonSerializer.Deserialize<DynamoDbSettings>(setting);
            return dbSetting;
        }

        public async Task PublishCourseAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<Contract.Models.Course> GetAsync(string id)
        {
            var configurations = new List<CourseDataobject>();
            var dynamoDbSettings = await GetDynamoDbSetting();

            var client = await _dynamoDbClientFactory.GetClientAsync(dynamoDbSettings);
            try
            {
                using (var context = _dynamoDbContextProvider.GetDynamoDbContext(client))
                {
                    var queryConfig = new QueryConfig
                    {
                        OverrideTableName = dynamoDbSettings.TableName
                    };

                    var search = context.QueryAsync<CourseDataobject>(id, queryConfig);

                    do
                    {
                        List<CourseDataobject> responseItems = await search.GetNextSetAsync();
                        configurations.AddRange(responseItems);
                    } while (!search.IsDone);
                }
            }
            catch (AmazonDynamoDBException ex)
            {
                throw new Exception("Something went wrong try again later");
            }


            return configurations;
        }

        public Task<List<Contract.Models.Course>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Contract.Models.Course> UpdateCourseAsync(Contract.Models.Course course)
        {
            throw new NotImplementedException();
        }
    }
}
