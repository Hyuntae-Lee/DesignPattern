1. This is an application to manage multiple projects and shared developers (resources) across the projects.
2. Each developer gets tasks from multiple projects.
3. The application should support following views.
   - Time-based overview (timeline)
   - Per-developer workload view
   - Cross-project visibility
4. In time-based overview, following interfaces should be available.
   - Add new project.
   - Remove an existing project.
   - Edit the name of existing project.
   - Edit the color of existing project.
   - Add new task to the existing project.
   - Remove an existing task.
   - Edit the name of existing task.
   - Edit the color of existing task.
   - Add sub-task to the existing task.
   - Add new resource.
   - Link tasks with following relation.
     - Dependency
       - One task cannot start before other specific tasks are finished.
5. GUI for time-line.
   - The time-line should be displayed as a grid.
   - It should display monthes and weeks with following layout.
     |  2  |  3  |  4  |
     | | | | | | | | | |
   - Assume that all monthes have four weeks.
6. GUI for task.
   - Each task should be displayed as a rectangle bar over the time-line grid.