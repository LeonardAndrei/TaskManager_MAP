# Task Manager - Aplicație C# (SOLID & N-Tier)

Acest proiect este o aplicație Windows Forms (.NET) pentru gestiunea sarcinilor, dezvoltată pentru a demonstra aplicarea corectă a celor 5 principii SOLID, arhitectura N-Tier, Dependency Injection și testarea unitară.

## Arhitectura și Dependențele (N-Tier)

Aplicația este împărțită în 3 straturi clare:
* **Core:** Inima aplicației. Conține logica de business (TaskService), validările, modelele și interfețele. Are **ZERO** dependențe externe.
* **Data:** Stratul de persistență (baza de date SQLite). Depinde de Core pentru a implementa interfețele.
* **UI:** Interfața grafică Windows Forms. Folosește Dependency Injection pentru a asambla aplicația.

Fluxul dependențelor:
[TaskManager.UI] ---> [TaskManager.Core] <--- [TaskManager.Data]

## Justificarea deciziilor de design SOLID

* **S - Single Responsibility Principle (SRP):** Am extras logica de validare în TaskValidator.cs. Astfel, TaskService doar coordonează fluxul de date, iar formularul MainForm.cs se ocupă exclusiv de afișare. Nicio clasă nu are două motive de schimbare.
* **O - Open/Closed Principle (OCP):** Sistemul de notificări (ITaskNotifier) permite adăugarea de noi metode de notificare fără a modifica logica existentă. Am demonstrat acest lucru adăugând TelegramNotifier (care trimite mesaje reale pe telefon prin API), pe care l-am injectat din exterior printr-un dicționar, lăsând clasa TaskService absolut neatinsă.
* **L - Liskov Substitution Principle (LSP):** Am folosit Design by Contract (Template Method). Clasa de bază abstractă TaskItem impune precondițiile (ex: statusul nu e deja Done) și postcondițiile în metoda Complete(). Clasele derivate (DeadlineTask, RecurringTask) respectă aceste reguli și sunt folosite interschimbabil inclusiv în interfața grafică (unde tabelul afișează dinamic termenul în funcție de tipul obiectului).
* **I - Interface Segregation Principle (ISP):** Am segregat ITaskRepository în două roluri distincte: ITaskReader (doar citire) și ITaskWriter (doar scriere). Astfel, ReportService (care generează sumarul din bara de titlu) depinde exclusiv de ITaskReader, făcând imposibilă tehnic ștergerea sau modificarea accidentală a datelor.
* **D - Dependency Inversion Principle (DIP):** Am eliminat instanțierile rigide folosind un Container de Inversiune a Controlului (IoC - Microsoft.Extensions.DependencyInjection). Program.cs acționează ca un Composition Root, asamblând dependențele. Modulele de nivel înalt (TaskService, MainForm) depind acum de abstracții, nu de implementări concrete.

## Repository Pattern și Testare

Folosim InMemoryTaskRepository pentru unit testing izolat (teste NUnit cu verde pentru validări și comportament) și SqliteTaskRepository pentru salvarea persistentă în mediul de producție. Datorită DIP, cele două pot fi schimbate cu ușurință printr-o singură linie de cod în containerul IoC.