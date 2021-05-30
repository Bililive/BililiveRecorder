using System;
using System.Linq;
using BililiveRecorder.Core;
using BililiveRecorder.Web.Schemas.Types;
using GraphQL;
using GraphQL.Types;

namespace BililiveRecorder.Web.Schemas
{
    internal class RecorderMutation : ObjectGraphType
    {
        private readonly IRecorder recorder;

        public RecorderMutation(IRecorder recorder)
        {
            this.recorder = recorder ?? throw new ArgumentNullException(nameof(recorder));

            this.SetupFields();
        }

        private void SetupFields()
        {
            this.Field<RoomType>("addRoom",
                arguments: new QueryArguments(
                    new QueryArgument<IntGraphType> { Name = "roomId" },
                    new QueryArgument<BooleanGraphType> { Name = "autoRecord" }
                    ),
                resolve: context =>
                {
                    var id = context.GetArgument<int>("roomId");
                    var enabled = context.GetArgument<bool>("autoRecord");
                    return this.recorder.AddRoom(id, enabled);
                });

            this.Field<RoomType>("removeRoom",
                arguments: new QueryArguments(
                    new QueryArgument<GuidGraphType> { Name = "objectId" },
                    new QueryArgument<IntGraphType> { Name = "roomId" }
                    ),
                resolve: context =>
                {
                    var objectId = context.GetArgument<Guid>("objectId");
                    var roomid = context.GetArgument<int>("roomId");

                    var room = objectId != default
                        ? this.recorder.Rooms.FirstOrDefault(x => x.ObjectId == objectId)
                        : this.recorder.Rooms.FirstOrDefault(x => x.RoomConfig.RoomId == roomid || x.ShortId == roomid);

                    if (room is not null)
                        this.recorder.RemoveRoom(room);
                    else
                        context.Errors.Add(new ExecutionError("Room not found"));

                    return room;
                });

            this.FieldAsync<RoomType>("refreshRoomInfo",
                arguments: new QueryArguments(
                    new QueryArgument<GuidGraphType> { Name = "objectId" },
                    new QueryArgument<IntGraphType> { Name = "roomId" }
                    ),
                resolve: async context =>
                {
                    var objectId = context.GetArgument<Guid>("objectId");
                    var roomid = context.GetArgument<int>("roomId");

                    var room = objectId != default
                        ? this.recorder.Rooms.FirstOrDefault(x => x.ObjectId == objectId)
                        : this.recorder.Rooms.FirstOrDefault(x => x.RoomConfig.RoomId == roomid || x.ShortId == roomid);

                    if (room is not null)
                        await room.RefreshRoomInfoAsync().ConfigureAwait(false);
                    else
                        context.Errors.Add(new ExecutionError("Room not found"));

                    return room;
                });

            this.Field<RoomType>("startRecording",
                arguments: new QueryArguments(
                    new QueryArgument<GuidGraphType> { Name = "objectId" },
                    new QueryArgument<IntGraphType> { Name = "roomId" }
                    ),
                resolve: context =>
                {
                    var objectId = context.GetArgument<Guid>("objectId");
                    var roomid = context.GetArgument<int>("roomId");

                    var room = objectId != default
                        ? this.recorder.Rooms.FirstOrDefault(x => x.ObjectId == objectId)
                        : this.recorder.Rooms.FirstOrDefault(x => x.RoomConfig.RoomId == roomid || x.ShortId == roomid);

                    if (room is not null)
                        room.StartRecord();
                    else
                        context.Errors.Add(new ExecutionError("Room not found"));

                    return room;
                });

            this.Field<RoomType>("stopRecording",
                arguments: new QueryArguments(
                    new QueryArgument<GuidGraphType> { Name = "objectId" },
                    new QueryArgument<IntGraphType> { Name = "roomId" }
                    ),
                resolve: context =>
                {
                    var objectId = context.GetArgument<Guid>("objectId");
                    var roomid = context.GetArgument<int>("roomId");

                    var room = objectId != default
                        ? this.recorder.Rooms.FirstOrDefault(x => x.ObjectId == objectId)
                        : this.recorder.Rooms.FirstOrDefault(x => x.RoomConfig.RoomId == roomid || x.ShortId == roomid);

                    if (room is not null)
                        room.StopRecord();
                    else
                        context.Errors.Add(new ExecutionError("Room not found"));

                    return room;
                });

            this.Field<RoomType>("splitRecordingOutput",
                arguments: new QueryArguments(
                    new QueryArgument<GuidGraphType> { Name = "objectId" },
                    new QueryArgument<IntGraphType> { Name = "roomId" }
                    ),
                resolve: context =>
                {
                    var objectId = context.GetArgument<Guid>("objectId");
                    var roomid = context.GetArgument<int>("roomId");

                    var room = objectId != default
                        ? this.recorder.Rooms.FirstOrDefault(x => x.ObjectId == objectId)
                        : this.recorder.Rooms.FirstOrDefault(x => x.RoomConfig.RoomId == roomid || x.ShortId == roomid);

                    if (room is not null)
                        room.SplitOutput();
                    else
                        context.Errors.Add(new ExecutionError("Room not found"));

                    return room;
                });

            this.Field<RoomType>("setRoomConfig",
                arguments: new QueryArguments(
                    new QueryArgument<GuidGraphType> { Name = "objectId" },
                    new QueryArgument<IntGraphType> { Name = "roomId" },
                    new QueryArgument<SetRoomConfigType> { Name = "config" }
                    ),
                resolve: context =>
                {
                    var objectId = context.GetArgument<Guid>("objectId");
                    var roomid = context.GetArgument<int>("roomId");
                    var config = context.GetArgument<SetRoomConfig>("config");

                    if (config is null)
                    {
                        context.Errors.Add(new ExecutionError("config can't be null"));
                        return null;
                    }

                    var room = objectId != default
                        ? this.recorder.Rooms.FirstOrDefault(x => x.ObjectId == objectId)
                        : this.recorder.Rooms.FirstOrDefault(x => x.RoomConfig.RoomId == roomid || x.ShortId == roomid);

                    if (room is null)
                        context.Errors.Add(new ExecutionError("Room not found"));
                    else
                        config.ApplyTo(room.RoomConfig);

                    return room;
                });

            this.Field<GlobalConfigType>("setConfig",
                arguments: new QueryArguments(
                    new QueryArgument<SetGlobalConfigType> { Name = "config" }
                    ),
                resolve: context =>
                {
                    var config = context.GetArgument<SetGlobalConfig>("config");

                    if (config is null)
                    {
                        context.Errors.Add(new ExecutionError("config can't be null"));
                        return null;
                    }

                    var recconfig = this.recorder.Config.Global;

                    config.ApplyTo(recconfig);

                    return recconfig;
                });
        }
    }
}
