using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System;

class Program
{
    private static string connectionString;

    static void Main(string[] args)
    {
        // Load configuration from appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        connectionString = configuration.GetConnectionString("dados");

        bool running = true;

        while (running)
        {
            Console.Clear();
            Console.WriteLine("Menu:");
            Console.WriteLine("1. Inserir nova tarefa");
            Console.WriteLine("2. Listar tarefas");
            Console.WriteLine("3. Atualizar tarefa");
            Console.WriteLine("4. Excluir tarefa");
            Console.WriteLine("5. Sair");
            Console.Write("Escolha uma opção: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    InsertTask();
                    break;
                case "2":
                    ListTasks();
                    break;
                case "3":
                    UpdateTask();
                    break;
                case "4":
                    DeleteTask();
                    break;
                case "5":
                    running = false;
                    Console.WriteLine("Saindo...");
                    break;
                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }

            if (running)
            {
                Console.WriteLine("Pressione qualquer tecla para continuar...");
                Console.ReadKey();
            }
        }
    }

    private static void InsertTask()
    {
        Console.Write("Digite a tarefa: ");
        var tarefa = Console.ReadLine();
        Console.WriteLine("Digite a data da tarefa (yyyy-mm-dd): ");
        var data = Console.ReadLine();

        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = "INSERT INTO cad_tarefas(tarefa, data_tarefa) VALUES (@tarefa, @data)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@tarefa", tarefa);
                    command.Parameters.AddWithValue("@data", DateTime.Parse(data));
                    command.ExecuteNonQuery();
                }
            }

            Console.WriteLine("Tarefa inserida com sucesso.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao inserir tarefa: {ex.Message}");
        }
    }

    private static void ListTasks()
    {
        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM cad_tarefas";
                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    Console.WriteLine("ID\tTarefa\t\tData");
                    Console.WriteLine("-----------------------------");
                    while (reader.Read())
                    {
                        var id = reader.GetInt32("id");
                        var tarefa = reader.GetString("tarefa");
                        var data = reader.GetDateTime("data_tarefa").ToString("yyyy-MM-dd");
                        Console.WriteLine($"{id}\t{tarefa}\t\t{data}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao listar tarefas: {ex.Message}");
        }
    }

    private static void UpdateTask()
    {
        Console.Write("Digite o ID da tarefa que deseja atualizar: ");
        var id = Console.ReadLine();
        Console.Write("Digite a nova tarefa: ");
        var novaTarefa = Console.ReadLine();
        Console.WriteLine("Digite a nova data da tarefa (yyyy-mm-dd): ");
        var novaData = Console.ReadLine();

        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = "UPDATE cad_tarefas SET tarefa = @novaTarefa, data_tarefa = @novaData WHERE id = @id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@novaTarefa", novaTarefa);
                    command.Parameters.AddWithValue("@novaData", DateTime.Parse(novaData));
                    command.Parameters.AddWithValue("@id", id);
                    var rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Tarefa atualizada com sucesso.");
                    }
                    else
                    {
                        Console.WriteLine("Nenhuma tarefa encontrada com esse ID.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar tarefa: {ex.Message}");
        }
    }

    private static void DeleteTask()
    {
        Console.Write("Digite o ID da tarefa que deseja excluir: ");
        var id = Console.ReadLine();

        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = "DELETE FROM cad_tarefas WHERE id = @id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    var rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Tarefa excluída com sucesso.");
                    }
                    else
                    {
                        Console.WriteLine("Nenhuma tarefa encontrada com esse ID.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao excluir tarefa: {ex.Message}");
        }
    }
}

