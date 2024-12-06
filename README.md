# Description
A cloud-based flashcard application designed to improve learning through spaced repetition, leveraging the principles of the Ebbinghaus Forgetting Curve. 
This project is developed as part of a cloud computing class, focusing on the integration of real-time notifications, secure user authentication, and scalable cloud technologies.


## Features
- __Space Repetition:__ Uses the Ebbinghaus forgetting curve to set review intervals for each flashcard set, helping users remember information effectively over time.
- __Real-time Notifications:__ Flashcard sets that are ready for review will automatically appear in the review section, utilizing real-time web sockets.
- __User Authentication:__ Secure login and registration with Azure AD-based identity management.

## Technologies Used
- __Frontend:__ `ASP.NET Core MVC` with `Razor` views for UI rendering.
- __Backend:__ `ASP.NET Core Web API` to handle CRUD operations for flashcard sets and review logic.
- __Database:__ `Azure SQL Database`, with support for both application data and Identity data.
- __Real-time Communication:__ `Azure SignalR Service` to push notifications to the client when flashcard sets are ready for review.
- __Cloud Platform:__ Azure service including `Azure App Service` for hosting, `Azure SQL` for storage, `Azure Active Directory` for secure authentication, and `Azure Container Registry` for application deployment.


## How Does the App Work?
The app leverages the concept of **spaced repetition** to ensure effective learning and memory retention. Users create flashcard sets, and the app automatically schedules review sessions at specific intervals. Flashcards needing review will appear in real-time on the app's home page, ensuring users do not miss any review sessions.

___The main logic of the review process is as follows:___
- When a new flashcard set is created, it is initially marked as needing immediate review.
- As users review the flashcard sets, the app tracks the stage of each set and schedules the next review according to an interval pattern that is progressively extended.
- When the next review interval is reached, the flashcard set will automatically appear in the "To Review" section of the application, leveraging **SignalR** to provide a real-time experience without the need for manual refreshes.
- The process repeats, and once all intervals are completed, the cycle resets to ensure long-term retention.

### Review Intervals
The app schedules reviews of flashcard sets based on the following intervals:

| Review Stage       | Interval      | Next Review After   |
|--------------------|---------------|---------------------|
| Initial Learning   | 0             | Immediately         |
| Stage 0            | 20 minutes    | 20 minutes later    |
| Stage 1            | 1 hour        | 1 hour later        |
| Stage 2            | 6 hours       | 6 hours later       |
| Stage 3            | 12 hours      | 12 hours later      |
| Stage 4            | 1 day         | 1 day later         |
| Stage 5            | 2 days        | 2 days later        |
| Stage 6            | 4 days        | 4 days later        |
| Stage 7            | 7 days        | 7 days later        |
| Stage 8            | 15 days       | 15 days later       |
| Stage 9            | 30 days       | 30 days later       |

Once all stages are completed, the review cycle resets to help reinforce long-term memory retention. Each flashcard set will appear in the **"To Review"** section when it is due for a review, without requiring the user to manually refresh the page.

> **Note:** In the current version, each flashcard set must contain at least one flashcard to ensure the review functionality works correctly. Attempting to review an empty set will result in a "Page Not Found" error.

This application demonstrates the effective use of cloud technologies and integration for developing a modern, scalable, and interactive educational tool.

## Setup Instructions
To set up the **Ebbinghaus Flashcard App** locally or in the cloud, follow these steps:

1. **Clone the Repository**
   ```bash
   git clone https://github.com/imyhalex/EbbinghausFlashcardApp.git
   ```

2. **Install Dependencies**
   - Ensure that you have **.NET SDK (version 8.0 or higher)** installed.
   - Install any required NuGet packages.

3. **Configure the Database**
   - Update the `appsettings.json` file with your Azure SQL Database connection string.
   - Ensure you have set up Azure Active Directory (Microsoft Entra ID) to provide access to the Azure SQL Database.

4. **Migrate the Database**
   ```bash
   dotnet ef database update
   ```

5. **Run the Application**
   ```bash
   dotnet run
   ```
   The app will be available at `https://localhost:5001` by default.

6. **Deploy to Azure**
   - You can publish the app to Azure using **Visual Studio** or the **Azure CLI**.
   - Ensure that you have set appropriate permissions for managed identity access to the Azure SQL Database.

## Demo Video & Live Website
**Video Demo** 
[![Watch the video](https://img.youtube.com/vi/_tNK3j13n6Y/default.jpg)](https://youtu.be/_tNK3j13n6Y)
___Live Website:___ [Website URL](https://ebbinghausflashcardapp20241204184808.azurewebsites.net/)

