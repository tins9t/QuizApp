﻿using Dapper;
using infrastructure.QueryModels;

namespace infrastructure.Repositories;

public class PasswordHashRepository
{
    public void CreatePasswordHash(string userId, string passwordHash, string salt)
    {
        Console.WriteLine($"Received userId: {userId}");
        var sql = $@"INSERT INTO security(user_id, password_hash, salt) VALUES (@userId, @passwordHash, @salt)";
        using var conn = DataConnection.DataSource.OpenConnection();
        conn.Execute(sql, new { userId, passwordHash, salt });
    }
    public void UpdatePassword(string userId, string newPassword, string newSalt)
    {
        var sql = $@"UPDATE security SET password_hash = @passwordHash, salt = @salt WHERE user_id = @id;";
        using var conn = DataConnection.DataSource.OpenConnection();
        conn.Execute(sql, new
        {
            id = userId,
            passwordHash = newPassword,
            salt = newSalt
        });
    }

    public PasswordHash GetPasswordHashByUserId(string userId)
    {
        var sql = $@"SELECT 
                    security.password_hash AS {nameof(PasswordHash.Hash)}, 
                    security.salt AS {nameof(PasswordHash.Salt)},
                    security.user_id AS {nameof(PasswordHash.UserId)}
                 FROM security 
                 WHERE user_id = @id;";
    
        using (var conn = DataConnection.DataSource.OpenConnection())
        {
            return conn.QueryFirst<PasswordHash>(sql, new { id = userId });
        }
    }


    public PasswordHash GetByEmail(string email)
    {
        const string sql = $@"
        SELECT 
        security.user_id as {nameof(PasswordHash.UserId)},
        security.password_hash as {nameof(PasswordHash.Hash)},
        security.salt as {nameof(PasswordHash.Salt)}
        FROM security
        JOIN users ON security.user_id = users.id
        WHERE email = @email;";
        using var connection = DataConnection.DataSource.OpenConnection();
        return connection.QuerySingle<PasswordHash>(sql, new { email });
    }
    
    public bool DeletePasswordHash(string userId)
    {
        const string sql = $@"DELETE FROM security WHERE user_id = @userId";
        using var connection = DataConnection.DataSource.OpenConnection();
        return connection.Execute(sql, new { userId }) > 0;
    }
}
