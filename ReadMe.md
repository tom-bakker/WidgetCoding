# Widget Coding

An example project by Thomas Bakker using an ASP.NET Core Backend with a Vue.JS Frontend.

## Local Installation and Setup

This is to run the application locally.

I have run this in Visual Studio 2022 version 17.14.11 and the startup settings are configured to launch 
first the server then the client. 

### Prerequisites

- Microsoft SQL Server Express : https://www.microsoft.com/en-us/sql-server/sql-server-downloads

- Node JS : https://nodejs.org/en/download

### First Run

The first run will run a migration creating the database 'sqldb-widgetcoding-test' with the table required and seed
some test data initially. 

## Technical Approach

I have used a clean architecture layers, core for the entities and other objects used through out each layer. 
The infrastucture layer for the database access, using Entity framework core. This is to enable me to be consistent
with the Widget model 
from the core to the server, also for other objects such as the WidgetCategories. 

I have used Bootstrap-Vue-Next for the styling of the webpages and componenets in the web application. 
I originally used Bootstrap-Vue, however due to its incompatibility with Vue 3 and Bootstrap 5, moved to 
Boostrap-Vue-Next as recommended by the contributors to Bootstrap-Vue. 

## Assumptions and Decisions

1. Date added to be stored as UTC.

This is an assumption made for better localization for the users, based off previous experiences I've had with 
dates and times. 

2. Enumeration of the Widget Categories.

I chose enumeration for the widget category for better comparison if filtering was required, in hindsight now 
having this as a list of constant strings would have been better for the size of the application. 

3. Use of modals for create, detail and edit forms. 

For a single page user experience, I leaned towards modals for the create and edit forms for widgets. 
This is due to the small amount of properties for a widget, enabling the user to add or edit a widget and 
have an immediate result on the application. 
