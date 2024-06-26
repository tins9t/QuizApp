﻿using Dapper;
using infrastructure.QueryModels;
namespace infrastructure.Repositories;

public class QuizRepository
{
    public Quiz CreateQuiz(Quiz quiz)
    {
        var sql = $@"INSERT INTO quiz(id, name, description, time_created, user_id, is_private)
        VALUES (@id, @name, @description, @time_created, @user_id, @is_private) RETURNING 
        id as {nameof(Quiz.Id)},
        name as {nameof(Quiz.Name)},
        description as {nameof(Quiz.Description)},
        time_created as {nameof(Quiz.TimeCreated)},
        user_id as {nameof(Quiz.UserId)},
        is_private as {nameof(Quiz.IsPrivate)};";
        using (var conn = DataConnection.DataSource.OpenConnection())
        {
            return conn.QueryFirst<Quiz>(sql, new
            {
                id = Guid.NewGuid().ToString(),
                name = quiz.Name,
                description = quiz.Description,
                time_created = DateTime.Today,
                user_id = quiz.UserId,
                is_private = true
            });
        }
    }

    public Quiz UpdateQuiz(Quiz quiz)
    {
        var sql = $@"UPDATE quiz SET
    {nameof(Quiz.Name)} = @name,
    {nameof(Quiz.Description)} = @description,
    is_private = @is_private
    WHERE {nameof(Quiz.Id)} = @id;";
        using (var conn = DataConnection.DataSource.OpenConnection())
        {
            conn.Execute(sql, new
            {
                name = quiz.Name,
                description = quiz.Description,
                user_id = quiz.UserId,
                is_private = quiz.IsPrivate,
                id = quiz.Id
            });
        }
        return GetQuizById(quiz.Id);
    }

    public bool DeleteQuizById(string quizId)
    {
        var sql = $@"DELETE FROM quiz WHERE id = @id;";
        using (var conn = DataConnection.DataSource.OpenConnection())
        {
            return conn.Execute(sql, new { id = quizId }) == 1;
        }
    }

    public Quiz GetQuizById(string id)
    {
        var sql = $@"SELECT id as {nameof(Quiz.Id)},
        name as {nameof(Quiz.Name)},
        description as {nameof(Quiz.Description)},
        time_created as {nameof(Quiz.TimeCreated)},
        user_id as {nameof(Quiz.UserId)},
        is_private as {nameof(Quiz.IsPrivate)} FROM quiz WHERE id = @id;"; 
        using (var conn = DataConnection.DataSource.OpenConnection())
        {
            return conn.QueryFirst<Quiz>(sql, new { id });
        }
    }

    public List<Quiz> GetNewestQuizzes()
    {
        var sql = $@"SELECT id as {nameof(Quiz.Id)},
        name as {nameof(Quiz.Name)},
        description as {nameof(Quiz.Description)},
        time_created as {nameof(Quiz.TimeCreated)},
        user_id as {nameof(Quiz.UserId)},
        is_private as {nameof(Quiz.IsPrivate)} 
        FROM quiz 
        WHERE is_private = false
        ORDER BY time_created DESC;";
        using (var conn = DataConnection.DataSource.OpenConnection())
        {
            return conn.Query<Quiz>(sql).ToList();
        }
    }

    public List<Quiz> GetQuizzesByUser(string userId)
    {
        var sql = $@"SELECT id as {nameof(Quiz.Id)},
        name as {nameof(Quiz.Name)},
        description as {nameof(Quiz.Description)},
        time_created as {nameof(Quiz.TimeCreated)},
        user_id as {nameof(Quiz.UserId)},
        is_private as {nameof(Quiz.IsPrivate)} FROM quiz WHERE user_id = @user_id;";
        using (var conn = DataConnection.DataSource.OpenConnection())
        {
            return conn.Query<Quiz>(sql, new { user_id = userId }).ToList();
        }
    }

    public List<Quiz> GetQuizzesByName(string name)
    {
        var sql = $@"SELECT id as {nameof(Quiz.Id)},
        name as {nameof(Quiz.Name)},
        description as {nameof(Quiz.Description)},
        time_created as {nameof(Quiz.TimeCreated)},
        user_id as {nameof(Quiz.UserId)},
        is_private as {nameof(Quiz.IsPrivate)} 
        FROM quiz 
        WHERE name ILIKE @name
        AND is_private = false;";
        using (var conn = DataConnection.DataSource.OpenConnection())
        {
            return conn.Query<Quiz>(sql, new { name = $"%{name}%" }).ToList();
        }
    }

    public List<Quiz> GetQuizzesByPopularity()
    {
        var sql = $@"SELECT q.id AS {nameof(Quiz.Id)}, 
            q.name AS {nameof(Quiz.Name)}, 
            q.description AS {nameof(Quiz.Description)}, 
            q.time_created AS {nameof(Quiz.TimeCreated)}, 
            q.user_id AS {nameof(Quiz.UserId)}, 
            q.is_private AS {nameof(Quiz.IsPrivate)}, 
            COUNT(qs.id) AS session_count
            FROM quiz q
            LEFT JOIN quiz_session qs ON q.id = qs.quiz_id
            WHERE q.is_private = false
            GROUP BY q.id, q.name, q.description, q.time_created, q.user_id, q.is_private
            ORDER BY session_count DESC;";
        using (var conn = DataConnection.DataSource.OpenConnection())
        {
            return conn.Query<Quiz>(sql).ToList();
        }
    }
    
    public string AddQuizSession(string quizId)
    {
        var sql = $@"INSERT INTO quiz_session(id, quiz_id)
        VALUES (@id, @quiz_id) RETURNING id;";
        using var conn = DataConnection.DataSource.OpenConnection();
        return conn.QueryFirst<string>(sql, new
        {
            id = Guid.NewGuid().ToString(),
            quiz_id = quizId,
        });
    }
    public bool DeleteQuizSession(string quizId)
    {
        var sql = $@"DELETE FROM quiz_session WHERE id = @id;";
        using var conn = DataConnection.DataSource.OpenConnection();
        return conn.Execute(sql, new { id = quizId }) == 1;
    }
}