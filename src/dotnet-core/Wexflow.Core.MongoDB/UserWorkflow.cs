﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Wexflow.Core.MongoDB
{
    public class UserWorkflow : Core.Db.UserWorkflow
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}
