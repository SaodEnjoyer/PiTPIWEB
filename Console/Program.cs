using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Model;
using ConsoleApp;

public class ApiSettings
{
    public string BaseAddress { get; set; } = string.Empty;
}

public interface IMusicApiService
{
    Task CreateAuthorAsync(Author author);
    Task<IEnumerable<Author>> GetAllAuthorsAsync();
    Task<Author?> GetAuthorByIdAsync(int id);
    Task UpdateAuthorAsync(int id, Author updatedAuthor);
    Task DeleteAuthorAsync(int id);
}

public class MusicApiService : IMusicApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MusicApiService> _logger;

    public MusicApiService(HttpClient httpClient, ILogger<MusicApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task CreateAuthorAsync(Author author)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("author", author);
            if (response.IsSuccessStatusCode)
                _logger.LogInformation("Автор успешно создан.");
            else
                _logger.LogError("Ошибка создания автора: {StatusCode}", response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании автора");
        }
    }

    public async Task<IEnumerable<Author>> GetAllAuthorsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Author>>("author") ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка получения списка авторов");
            return new List<Author>();
        }
    }

    public async Task<Author?> GetAuthorByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Author>($"author/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка получения автора по ID");
            return null;
        }
    }

    public async Task UpdateAuthorAsync(int id, Author updatedAuthor)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"author/{id}", updatedAuthor);
            if (response.IsSuccessStatusCode)
                _logger.LogInformation("Автор успешно обновлён.");
            else
                _logger.LogError("Ошибка обновления автора: {StatusCode}", response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении автора");
        }
    }

    public async Task DeleteAuthorAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"author/{id}");
            if (response.IsSuccessStatusCode)
                _logger.LogInformation("Автор успешно удалён.");
            else
                _logger.LogError("Ошибка удаления автора: {StatusCode}", response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении автора");
        }
    }
}

public class Program
{
    private readonly IServiceProvider _serviceProvider;

    public Program(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task RunAsync()
    {
        var musicService = _serviceProvider.GetRequiredService<IMusicApiService>();
        var logger = _serviceProvider.GetRequiredService<ILogger<Program>>();

        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\nВыберите действие:");
            Console.WriteLine("1. Создать автора");
            Console.WriteLine("2. Получить всех авторов");
            Console.WriteLine("3. Получить автора по ID");
            Console.WriteLine("4. Обновить автора");
            Console.WriteLine("5. Удалить автора");
            Console.WriteLine("6. Выйти");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Console.Write("Введите имя автора: ");
                    var name = Console.ReadLine();
                    await musicService.CreateAuthorAsync(new Author(0, name));
                    break;
                case "2":
                    var authors = await musicService.GetAllAuthorsAsync();
                    foreach (var author in authors)
                        Console.WriteLine($"{author.Id}: {author.Name}");
                    break;
                case "3":
                    Console.Write("Введите ID автора: ");
                    if (int.TryParse(Console.ReadLine(), out int id))
                    {
                        var author = await musicService.GetAuthorByIdAsync(id);
                        Console.WriteLine(author != null ? $"{author.Id}: {author.Name}" : "Автор не найден.");
                    }
                    break;
                case "4":
                    Console.Write("Введите ID автора для обновления: ");
                    if (int.TryParse(Console.ReadLine(), out int updateId))
                    {
                        Console.Write("Введите новое имя автора: ");
                        var updatedName = Console.ReadLine();
                        await musicService.UpdateAuthorAsync(updateId, new Author(updateId, updatedName));
                    }
                    break;
                case "5":
                    Console.Write("Введите ID автора для удаления: ");
                    if (int.TryParse(Console.ReadLine(), out int deleteId))
                        await musicService.DeleteAuthorAsync(deleteId);
                    break;
                case "6":
                    exit = true;
                    logger.LogInformation("Выход из программы...");
                    break;
                default:
                    Console.WriteLine("Некорректный выбор. Попробуйте снова.");
                    break;
            }
        }
    }

    public static async Task Main(string[] args)
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<IConfiguration>(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build());

        serviceCollection.AddHttpClient<IMusicApiService, MusicApiService>((provider, client) =>
        {
            var settings = provider.GetRequiredService<IOptions<ApiSettings>>().Value;
            client.BaseAddress = new Uri(settings.BaseAddress);
        });

        serviceCollection.Configure<ApiSettings>(config =>
        {
            config.BaseAddress = Conf.GetAdress(); 
        });

        serviceCollection.AddLogging(logging => logging.AddConsole());
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var program = new Program(serviceProvider);
        await program.RunAsync();
    }
}
