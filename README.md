# Task Manager - Laborator 3 (Principiile SOLID)

Acest proiect demonstreaza aplicarea primelor 3 principii SOLID in C#.

## Arhitectura
Aplicatia este impartita in 3 straturi clare (Core, Data, UI).

## Justificarea cerintelor SOLID:
1. **SRP (Single Responsibility):** Am extras logicile de validare in `TaskValidator.cs`. `TaskService` coordoneaza actiunile, iar `Program.cs` se ocupa doar de afisare in consola. Nicio clasa nu are doua motive de schimbare.
2. **OCP (Open/Closed):** Sistemul de notificari (`ITaskNotifier`) permite adaugarea de noi metode de notificare fara a modifica `TaskService.cs`, doar injectandu-le din exterior (via un Dictionar).
3. **LSP (Liskov Substitution):** Am folosit *Design by Contract* (Template Method). Clasa de baza abstracta `TaskItem` impune preconditiile (Status nu e deja Done) si postconditiile in metoda `Complete()`. Clasele derivate (`DeadlineTask`, `RecurringTask`) moștenesc si respecta aceste reguli, adaugand doar comportament propriu in `CompleteCore()`.

## Repository Pattern
Folosim `InMemoryTaskRepository` pentru unit testing izolat (NUnit) si `SqliteTaskRepository` pentru salvarea persistenta in mediul de productie. Cele doua pot fi schimbate cu usurinta.