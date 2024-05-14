# **DiaryProject**
Проект представляет собой Web Api для дневника, который позволяет пользователям заполнять отчёта и сохранять, редактировать и удалять их.

# Технологический стек проекта:
* ASP.Net core
*  Entity Framework core
*  AutoMapper
*  FluentValidation
* JWT Bearer для аутентификации
*  Serilog для логгирования
*  RabbitMQ - брокер сообщений

# Проект разделён на три слоя:
* Presentation - WebApi (REST API)
* Infrastructure - Работа с DBContext, брокерами сообщений
* Core - ядро проекта (Application для сервисов, выступающих посредниками между контроллерами и базами данных, Domain для связанных с проектом Entity и DTO).
* В проекте также используется Dependency Injection для регистрации всех зависимостей для конфигурации Web Api в одном месте.

# Разделы проекта
* Diary.Domain - содержит описание сущностей, DTO, Интерфейсов а также некоторых вспомогательных классов.

## Сущности
```
public class User : IEntityId<long>, IAuditable
    {
        public long Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public List<Report> Reports { get; set; }
        public List<Role> Roles { get; set; }
        public UserToken UserToken { get; set; }
        public DateTime CreatedAt { get; set; }
        public long CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public long? UpdatedBy { get; set; }
    }
```

