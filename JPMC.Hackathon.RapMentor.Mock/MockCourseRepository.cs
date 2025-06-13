//using JPMC.Hackathon.RapMentor.Adapter.Dynamodb;
//using JPMC.Hackathon.RapMentor.Contract.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace JPMC.Hackathon.RapMentor.Mock
//{
//    public class MockCourseRepository : ICourseRepository
//    {
//        private static List<Course> _courseList = new List<Course>();

//        public async Task<Course> CreateCourseAsync(Course course)
//        {
//            var courseToSave = new Course
//            {
//                Id = Guid.NewGuid().ToString(),
//                Level = course.Level,
//                Description = course.Description,
//                Title = course.Title,
//                AuthorId = course.AuthorId,
//            };

//            _courseList.Add(course);
//            return courseToSave;
//        }

//        public async Task<List<Course>> GetAllAsync()
//        {
//            return GetAllCourse();
//        }

//        public async Task<Course?> GetAsync(string id)
//        {
//            return GetAllCourse().FirstOrDefault(x => x.Id == id);
//        }

//        public async Task PublishCourseAsync(string id)
//        {
//            var course = await GetAsync(id);
//            if (course != null)
//            {
//                course.CourseStatus = CourseStatus.Published.ToString();
//            }
//        }

//        private static List<Course> GetAllCourse()
//        {
//            if (_courseList.Count > 0)
//            {
//                return _courseList;
//            }
//            else
//            {
//                var fakeCourse = new FakeCourse();
//                _courseList = fakeCourse.Generate(10);
//                return _courseList;
//            }
//        }

//        public Task<Course> UpdateCourseAsync(Course course)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
