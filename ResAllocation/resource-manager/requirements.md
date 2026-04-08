1. This is an application to manage multiple projects and shared resources across the projects.
2. Each resource gets tasks from multiple projects.
3. The application should support following views.
   - Time-based overview (timeline)
   - Project managing view
   - Resource managing view
   - Task managing view
   - Resource cross tasks view
4. Time-based overview
   - The class name for this view should be TimeBasedOverview.
   - The view should be located the center of the main screen.
   - The view should fill extra space of the screen except side panels.
   - The view should be displayed as a grid.
   - It should display the time-line with monthes and weeks in following layout.
     |  2  |  3  |  4  |
     | | | | | | | | | |
   - Assume that all monthes have four weeks.
   - The time-line should be placed at the top of the view.
   - The time-line should be located fixed position regardless vertical scrolling.
   - Below the time line, the view should fill the background with grid view vertically aligned with the time-line.
   - Tasks should be overlayed the grid matching it's period.
   - The projects should be displayed at the left out of the time-line.
   - It should be aligned horizontal lines of the main grid to include it's tasks.
5. Project managing view
   - The class name for this view should be ProjectManagingView.
   - This view should be displayed in the left panel of the main display.
   - In this view, the user should be able to operate following functions.
     - Add new project.
     - Remove existing project.
     - Edit the name of existing project.
     - Edit the color of existing project.
     - Edit the period of existing project.
6. Resource managing view
   - The class name for this view should be ResourceManagingView.
   - This view should be displayed in the right panel of the main display.
   - In this view, the user should be able to operate following functions.
     - Add new resource.
     - Remove existing resource.
     - Edit the name of existing resource.
     - Edit the color of existing resource.
7. Task managing view by right clicking a project.
   - The class name for this view should be TaskManagingView
   - This view should be shown as a popup by right clicking a project in the TimeBasedOverview.
   - This view should support following functions.
     - List up all tasks in the clicked project.
     - Add new task to the clicked project.
     - Edit the name of existing task in the clicked project.
     - Edit the color of existing task.
     - Edit the period of existing task.
8. Task managing view by right clicking a task.
   - The class name for this view should be TaskManagingView
   - This view should be shown as a popup by right clicking a task in the TimeBasedOverview.
   - This view should support following functions.
     - List up all sub-tasks in the clicked task.
     - Add new sub-task to the clicked task.
     - Edit the name of existing sub-task in the clicked task.
     - Edit the color of existing sub-task.
     - Edit the period of existing sub-task.
