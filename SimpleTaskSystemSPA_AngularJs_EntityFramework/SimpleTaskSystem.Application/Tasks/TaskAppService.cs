﻿using System.Collections.Generic;
using System.Threading;
using Abp.Domain.Uow;
using AutoMapper;
using Castle.Core.Logging;
using SimpleTaskSystem.People;
using SimpleTaskSystem.Tasks.Dtos;

namespace SimpleTaskSystem.Tasks
{
    public class TaskAppService : ITaskAppService
    {
        public ILogger Logger { get; set; }

        private readonly ITaskRepository _taskRepository;
        private readonly IPersonRepository _personRepository;

        public TaskAppService(ITaskRepository taskRepository, IPersonRepository personRepository)
        {
            _taskRepository = taskRepository;
            _personRepository = personRepository;
            Logger = NullLogger.Instance;
        }

        public GetTasksOutput GetTasks(GetTasksInput input)
        {
            var tasks = _taskRepository.GetAllWithPeople(input.AssignedPersonId, input.State);
            return new GetTasksOutput
                   {
                       Tasks = Mapper.Map<List<TaskDto>>(tasks)
                   };
        }

        public void UpdateTask(UpdateTaskInput input)
        {
            Logger.Info("Updating a task for input: " + input);

            var task = _taskRepository.Get(input.TaskId);

            if (input.State.HasValue)
            {
                task.State = input.State.Value;
            }

            if (input.AssignedPersonId.HasValue)
            {
                task.AssignedPerson = _personRepository.Load(input.AssignedPersonId.Value);
            }
        }

        public void CreateTask(CreateTaskInput input)
        {
            var task = new Task { Description = input.Description };

            if (input.AssignedPersonId.HasValue)
            {
                task.AssignedPersonId = input.AssignedPersonId.Value;
            }

            _taskRepository.Insert(task);
        }
    }
}