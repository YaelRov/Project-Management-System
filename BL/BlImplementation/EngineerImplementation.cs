﻿
namespace BlImplementation;
using BlApi;
using BO;
using System;

/// <summary>
/// class for the implementations of engineer methods
/// </summary>
internal class EngineerImplementation : IEngineer
{
    private DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// Creates new entity object of Engineer
    /// </summary>
    /// <param name="boEngineer">engineer of type BO.Engineer</param>
    /// <returns>the created engineer id</returns>
    /// <exception cref="BO.BlAlreadyExistsException"></exception>
    public int Create(BO.Engineer boEngineer)
    {
        DO.Engineer doEngineer = new DO.Engineer
        (boEngineer.Id,
        boEngineer.Name, 
        boEngineer.Email,
        (DO.EngineerExperience)boEngineer.Level,
        boEngineer.Cost);
        try
        {
            int idEng = _dal.Engineer.Create(doEngineer);
            if (boEngineer.Task is not null) {
                DO.Task? taskOfEng = _dal.Task.Read(boEngineer.Task.Id);
                if (taskOfEng != null)
                {
                    _dal.Task.Update(taskOfEng with { EngineerId = boEngineer.Id });
                }
             }
            return idEng;
        }
        catch (DO.DalAlreadyExistsException exception)
        {
            throw new BO.BlAlreadyExistsException($"An object of type Engineer with ID {boEngineer.Id} already exists", exception);
        }

    }

    /// <summary>
    /// Deletes an engineer by its Id
    /// </summary>
    /// <param name="id">id of engineer, type int</param>
    /// <exception cref="BO.BlDoesNotExistException"></exception>
    public void Delete(int id)
    {
        try
        {
            _dal.Engineer.Delete(id);
            var taskOfEng = (from task in _dal.Task.ReadAll()
                    where task.EngineerId == id
                    select task).First();
            if (taskOfEng is not null)
            {
                _dal.Task.Update(taskOfEng with { EngineerId = null });
            }
        }
        catch (DO.DalDoesNotExistException exception)
        {
            throw new BO.BlDoesNotExistException($"An object of type Engineer with ID {id} does not exist", exception);
        }
    }

    /// <summary>
    /// Reads engineer by its ID 
    /// </summary>
    /// <param name="id">engineer id, type int</param>
    /// <returns>the engineer entity</returns>
    /// <exception cref="BO.BlDoesNotExistException"></exception>
    public BO.Engineer? Read(int id)
    {
        DO.Engineer? doEngineer = _dal.Engineer.Read(id);
        if (doEngineer == null)
            throw new BO.BlDoesNotExistException($"An object of type Engineer with ID {id} does not exist");
        var tasks = _dal.Task.ReadAll();
        var engTask = (from task in tasks
                           where task.EngineerId==id //&& task.Start is not null&& task.Complete is null
                           select new { task.Id, task.Alias }).FirstOrDefault();
        TaskInEngineer? taskInEngineer= null;
        if (engTask is not null)
            taskInEngineer = new TaskInEngineer() { Id=engTask.Id, Alias=engTask.Alias };
        return new BO.Engineer()
        {
            Id = id,
            Name = doEngineer.Name,
            Email = doEngineer.Email,
            Level = (BO.EngineerExperience)doEngineer.Level,
            Cost = doEngineer.Cost,
            Task = taskInEngineer
        };
    }

    /// <summary>
    /// Reads engineer according to a condition
    /// </summary>
    /// <param name="filter">condition</param>
    /// <returns>the engineer entity</returns>
    public BO.Engineer? Read(Func<BO.Engineer, bool> filter)
    {
         return ReadAll(filter).First();
    }

    /// <summary>
    /// Reads all engineers
    /// </summary>
    /// <param name="filter"></param>
    /// <returns>IEnumerable<BO.Engineer?></returns>

    public IEnumerable<BO.Engineer?> ReadAll(Func<BO.Engineer, bool>? filter = null)
    {
        var doEngList = _dal.Engineer.ReadAll();
        List<BO.Engineer?> boEngList = new List<BO.Engineer?>();
        if (filter != null)
        {
            foreach (var engineer in doEngList)
            {
                var eng = Read(engineer!.Id)!;
                if (filter(eng)) 
                    boEngList.Add(eng);
            }
        }
        else
        {
            foreach (var engineer in doEngList)
            {
                boEngList.Add(Read(engineer!.Id)!);
            }
        }
        return boEngList;
    }

    /// <summary>
    /// Updates engineer
    /// </summary>
    /// <param name="boEngineer"></param>
    /// <exception cref="BO.BlDoesNotExistException"></exception>

    public void Update(BO.Engineer boEngineer)
    {
        DO.Engineer doEngineer = new DO.Engineer
       (boEngineer.Id, boEngineer.Name, boEngineer.Email, (DO.EngineerExperience)boEngineer.Level, boEngineer.Cost);
        try
        {
            _dal.Engineer.Update(doEngineer);
            if (boEngineer.Task is not null)
            {
                DO.Task? taskOfEng = _dal.Task.Read(boEngineer.Task.Id);
                if (taskOfEng != null)
                {
                    _dal.Task.Update(taskOfEng with { EngineerId = boEngineer.Id });
                }
            }

        }
        catch (DO.DalDoesNotExistException exception)
        {
            throw new BO.BlDoesNotExistException($"An object of type Engineer with ID {boEngineer.Id} does not exist", exception);
        }
    }
}


