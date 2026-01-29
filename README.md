# ![TvTracker](https://github.com/Desarrollo-De-Software-RB/App-BE/blob/master/angular/src/assets/images/logo/TvtrackerLogo.png?raw=true)
TvTracker is a web application designed to help users track their favorite TV series, discover new shows, and manage their watchlist. Built with a robust **.NET** backend using the **ABP Framework** and a dynamic **Angular** frontend, it integrates with the **OMDB API** to provide real-time information about TV shows.

## 👥 Team Members

- Felipe Palazzi
- Mauricio Nahuel Salto
- Leandro Fidel Ruano

## 🚀 Features

### 1. Series Search
*   **Search Series**: Users can search for TV series by title or genre using the external OMDB API.

### 2. Series Management
*   **Get Series Information**: Retrieves detailed information (title, genre, release date, duration, team, cover photo, country of origin, and IMDB rating) from the internal database.
*   **Persist Series Information**: Saves series details fetched from the API into the internal database for future access.

### 3. Watchlist
*   **View Watchlist**: Users can view the series currently in their watchlist.
*   **Add to Watchlist**: Allows users to add series to their watchlist to receive notifications about relevant updates.
*   **Remove from Watchlist**: Users can remove series from their watchlist.

### 4. Notifications
*   **On-Screen Notifications**: Displays notifications about changes to watchlist series on the main screen, differentiating between read and unread messages.
*   **Email Notifications**: Sends notifications via email to the user.
*   **Notification Settings**: Users can configure which types of notifications they wish to receive.
*   **Notification Generation**: A background system process periodically checks for updates and generates notifications for watchlist series, persisting them in the database.

### 5. Series Rating
*   **Rate Series**: Users can rate series on a scale of 1 to 5 stars and add optional comments.
*   **Edit Rating**: Users can modify their previous ratings and comments.

### 6. Authentication
*   **Login**: Secure login with username and password.

### 7. Administrative Features
*   **User Management**: Full access to user management functionalities.
*   **API Monitoring Panel**: Visualizes statistics such as API access count, response times, error rates, etc.
*   **Monitoring Log**: Records events in a log file for error diagnosis and debugging.

### 8. User Administration
*   **Register New User**: Admins can create new users with username, full name, password, and profile picture.
*   **Delete User**: Admins can remove existing users.
*   **View Users**: Admins can view full details of all users. Standard users can only see the names of other users.
*   **Edit Profile**: Users can update their own profile information (full name, password, profile picture).

## 🛠️ Technologies Used

*   **Backend**: .NET 8, ABP Framework
*   **Frontend**: Angular
*   **Database**: SQL Server (via Entity Framework Core)
*   **External API**: OMDB API

## 💻 How to Run the Application

### Prerequisites
*   [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet)
*   [Node.js](https://nodejs.org/en) (v18 or v20)
*   [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or LocalDB)

### Backend Setup

1.  **Clone the repository**.
2.  **Navigate to the backend directory**:
    The solution file `TvTracker.sln` is in the root directory.
3.  **Run Database Migrations**:
    Execute the `TvTracker.DbMigrator` project to set up the database and seed initial data.
    ```bash
    dotnet run --project src/TvTracker.DbMigrator/TvTracker.DbMigrator.csproj
    ```
4.  **Start the Backend API**:
    Run the `TvTracker.HttpApi.Host` project.
    ```bash
    dotnet run --project src/TvTracker.HttpApi.Host/TvTracker.HttpApi.Host.csproj
    ```

### Frontend Setup

1.  **Navigate to the Angular directory**:
    ```bash
    cd angular
    ```
2.  **Install Dependencies**:
    ```bash
    npm install
    ```
3.  **Start the Development Server**:
    ```bash
    npm start
    ```
    Typically, the application will be available at `http://localhost:4200`.

## 🧪 Testing and Monitoring

*   **Unit/Integration Tests**: The solution includes a `test` folder with automated tests. run `dotnet test` in the root directory to execute them.
*   **Background Jobs**: The notification generation process runs automatically in the background when the backend is running.
*   **Logs**: Check the `Logs` folder (usually inside `src/TvTracker.HttpApi.Host/Logs` or similar) for the monitoring bitacora/logs.
