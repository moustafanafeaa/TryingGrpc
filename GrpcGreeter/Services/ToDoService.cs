using Grpc.Core;
using GrpcGreeter.Data;
using GrpcGreeter.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcGreeter.Services
{
    public class ToDoService : ToDoIt.ToDoItBase
    {
        private readonly AppDbContext _dbContext;

        public ToDoService(AppDbContext context)
        {
            _dbContext = context;
        }

        public override async Task<CreateToDoResponse> CreateToDo(CreateToDoRequest request, ServerCallContext context)
        {
            if(request.Title ==  string.Empty || request.Description == string.Empty)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid object"));
            }

            var item = new ToDoItem
            {
                Title = request.Title,
                Description = request.Description,
            };
            await _dbContext.ToDoItems.AddAsync(item);
            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new CreateToDoResponse
            {
                Id = item.Id
            });
        }
        public override async Task<ReadToDoResponse> ReadToDo(ReadToDoRequest request,ServerCallContext context)
        {
            if(request.Id <= 0)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "index must be greater than 0"));
            }
            var toDoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(t => t.Id == request.Id)
                                ?? throw new RpcException(new Status(StatusCode.NotFound, $"No Task with id {request.Id}"));
         
            return await Task.FromResult(new ReadToDoResponse
            {
                Id = toDoItem.Id,
                Title = toDoItem.Title,
                Description = toDoItem.Description,
                Status = toDoItem.ToDoStatus,
            });
            
        }

        public override async Task<GetAllToDoResponse> GetAllToDo(GetAllToDoRequest request, ServerCallContext context)
        {
            var response = new  GetAllToDoResponse();
            var toDoItems = await _dbContext.ToDoItems.AsNoTracking().ToListAsync();
            foreach(var toDo in toDoItems)
            {
                response.ToDo.Add(new ReadToDoResponse
                {
                    Id = toDo.Id,
                    Title = toDo.Title,
                    Description =   toDo.Description,
                    Status = toDo.ToDoStatus,
                });
            }
            return await Task.FromResult(response);
        }

        public override async Task<UpdateToDoResponse> UpdateToDo(UpdateToDoRequest request, ServerCallContext context)
        {
            if (request.Title == string.Empty || request.Description == string.Empty || request.Id <= 0)
                 throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid object"));
            
            var toDoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(t => t.Id == request.Id)
                ?? throw new RpcException(new Status(StatusCode.NotFound, $"No Task with id {request.Id}"));
            toDoItem.Title = request.Title;
            toDoItem.Description = request.Description;
            toDoItem.ToDoStatus = request.Status;

            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new UpdateToDoResponse
            {
                Id = toDoItem .Id,
            });
        }

        public override async Task<DeleteToDoResponse> DeleteToDo(DeleteToDoRequest request, ServerCallContext context)
        {
            if (request.Id <= 0)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "index must be greater than 0"));

            var toDoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(t => t.Id == request.Id)
                ?? throw new RpcException(new Status(StatusCode.NotFound, $"No Task with id {request.Id}"));

            _dbContext.ToDoItems.Remove(toDoItem);
            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new DeleteToDoResponse
            {
                Id = toDoItem.Id
            });
        }

    }
}
