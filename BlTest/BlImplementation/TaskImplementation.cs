﻿
namespace BlTest.BlImplementation;
using BlApi;

internal class TaskImplementation : ITask
{
    private DalApi.IDal _dal = Factory.Get;
    public int Create(BO.Task boTask)
    {
        DO.Task doTask = new DO.Task
               (boTask.Id,
               boTask.Description,
               boTask.Alias,
               boTask.Milestone is not null? true:false,
               boTask.CreatedAt,
               boTask.Start, 
               boTask.ScheduledDate, 
               boTask.ForecastDate, 
               boTask.Deadline, 
               boTask.Complete, 
               boTask.Deliverables, 
               boTask.Remarks, 
               boTask.Engineer is not null?boTask.Engineer.Id: null,
               boTask.ComplexityLevel is not null?(DO.EngineerExperience)boTask.ComplexityLevel: null);
        try
        {
            int idTask = _dal.Task.Create(doTask);
            return idTask;
        }
        catch (DO.DalAlreadyExistsException exception)
        {
            throw new BO.BlAlreadyExistsException($"An object of type Task with ID {boTask.Id} already exists");//, exception);
        }
    }

    public void Delete(int id)
    {
        try
        {
            _dal.Task.Delete(id);
        }
        catch (DO.DalDoesNotExistException exception)
        {
            throw new BO.BlDoesNotExistException($"An object of type Task with ID {id} does not exist");
        }
    }

    public BO.Task? Read(int id)
    {
        DO.Task? doTask = _dal.Task.Read(id);
        if (doTask == null)
            throw new BO.BlDoesNotExistException($"An object of type Task with ID {id} does not exist");
        return new BO.Task()
        {
            Id= doTask.Id,
            Description=doTask.Description,
            Alias=doTask.Alias,
            Milestone=doTask.Milestone is true? new BO.MilestoneInTask() : null,
            CreatedAt=doTask.CreatedAt,
            Start=doTask.Start,
            ScheduledDate=doTask.ScheduledDate,
            ForecastDate=doTask.ForecastDate,
            Deadline=doTask.Deadline,
            Complete=doTask.Complete,
            Deliverables=doTask.Deliverables,
            Remarks=doTask.Remarks,
            Engineer=doTask.EngineerId is not null ? new BO.EngineerInTask() : null,
            ComplexityLevel =doTask.ComplexityLevel is not null ? (BO.EngineerExperience)doTask.ComplexityLevel : null
        };
    }

    public BO.Task? Read(Func<BO.Task, bool> filter)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.Task?> ReadAll(Func<BO.Task, bool>? filter = null)
    {
        return (from DO.Task doTask in _dal.Task.ReadAll()
                select new BO.Task
                {
                    Id = doTask.Id,
                    Description = doTask.Description,
                    Alias = doTask.Alias,
                    Milestone = doTask.Milestone is true ? new BO.MilestoneInTask() : null,
                    CreatedAt = doTask.CreatedAt,
                    Start = doTask.Start,
                    ScheduledDate = doTask.ScheduledDate,
                    ForecastDate = doTask.ForecastDate,
                    Deadline = doTask.Deadline,
                    Complete = doTask.Complete,
                    Deliverables = doTask.Deliverables,
                    Remarks = doTask.Remarks,
                    Engineer = doTask.EngineerId is not null ? new BO.EngineerInTask() : null,
                    ComplexityLevel = doTask.ComplexityLevel is not null ? (BO.EngineerExperience)doTask.ComplexityLevel : null
                });
    }

    public void Update(BO.Task boTask)
    {
        DO.Task doTask = new DO.Task
             (boTask.Id,
               boTask.Description,
               boTask.Alias,
               boTask.Milestone is not null ? true : false,
               boTask.CreatedAt,
               boTask.Start,
               boTask.ScheduledDate,
               boTask.ForecastDate,
               boTask.Deadline,
               boTask.Complete,
               boTask.Deliverables,
               boTask.Remarks,
               boTask.Engineer is not null ? boTask.Engineer.Id : null,
               boTask.ComplexityLevel is not null ? (DO.EngineerExperience)boTask.ComplexityLevel : null);
        try
        {
            _dal.Task.Update(doTask);
        }
        catch (DO.DalDoesNotExistException exception)
        {
            throw new BO.BlDoesNotExistException($"An object of type Task with ID {boTask.Id} does not exist");//, exception);
        }
    }
}
