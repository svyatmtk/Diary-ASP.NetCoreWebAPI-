# **DiaryProject**
Проект представляет собой Web Api для дневника, который позволяет пользователям заполнять отчёта и сохранять, редактировать и удалять их.

# Технологический стек проекта:
1.ASP.Net core
2. Entity Framework core
3. AutoMapper
4. FluentValidation
5.JWT Bearer для аутентификации
6. Serilog для логгирования
7. RabbitMQ - брокер сообщений

# Проект разделён на три слоя:
Presentation - WebApi (REST API)
Infrastructure - Работа с DBContext, брокерами сообщений
Core - ядро проекта (Application для сервисов, выступающих посредниками между контроллерами и базами данных, Domain для связанных с проектом Entity и DTO).
В проекте также используется Dependency Injection для регистрации всех зависимостей для конфигурации Web Api в одном месте.

