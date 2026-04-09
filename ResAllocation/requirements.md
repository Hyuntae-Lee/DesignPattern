# Project / Task / Resource Management Application Specification

## 1. Overview

This application manages multiple projects, tasks, and shared resources.

- A **Project** contains multiple tasks.
- A **Task** belongs to exactly one project and may contain sub-tasks.
- A **Resource** can be assigned to multiple tasks.
- A **Task** can have multiple assigned resources.

---

## 2. Main Views

The application consists of the following views:

- Time-based overview (central view)
- Project management view (left panel)
- Resource management view (right panel)
- Task management popup (context-sensitive)

---

## 3. Time-Based Overview

**Class name:** `TimeBasedOverview`

### Layout
- Positioned at the center of the main window
- Expands to fill available space between left and right panels
- Displayed as a **grid-based timeline (Gantt-style)**

---

### 3.1 Display Period Control

- A control UI should be provided to set the visible time range.
- The control should allow:
  - Selecting a **start date**
  - Selecting an **end date**
- The control should be located at the **top of the TimeBasedOverview**
- Changing the period updates the timeline and grid dynamically

---

### 3.2 Timeline

- Horizontal axis represents **time in days**
- Vertical axis represents projects and/or tasks

#### Timeline Header
- Displays dates (e.g., YYYY-MM-DD)
- May group days into weeks or months for readability
- Is fixed at the top during vertical scrolling

---

### 3.3 Grid

- Grid columns represent **individual days**
- Grid lines align with the timeline header
- Supports horizontal and vertical scrolling

---

### 3.4 Holiday Visualization

- Holidays should be visually distinguished in the grid
- The background of grid cells corresponding to holidays should be **filled with gray color**
- Holiday detection must be performed via the `HolidayProvider` interface
- Non-working days (e.g., weekends) may optionally be styled similarly

---

### 3.5 Task Display

- Tasks are displayed as horizontal bars spanning their start and end dates
- Tasks may overlap visually
- Each task uses its assigned color
- Task bars should align precisely with day-based grid cells

---

## 4. Project Management View

**Class name:** `ProjectManagingView`

### Layout
- Located in the left panel

### Features
- Create a new project
- Delete an existing project
- Edit project name
- Edit project color
- Edit project start and end dates

---

## 5. Resource Management View

**Class name:** `ResourceManagingView`

### Layout
- Located in the right panel

### Features
- Create a new resource
- Delete an existing resource
- Edit resource name
- Edit resource color

---

## 6. Task Management (Project Context)

**Class name:** `ProjectTaskManagingPopup`

### Trigger
- Right-click on a project in the TimeBasedOverview

### Features
- List all tasks in the selected project
- Create a new task
- Delete a task
- Edit task name
- Edit task color
- Edit task start and end dates
- Assign or remove resources to/from a task

---

## 7. Task Management (Task Context)

**Class name:** `TaskDetailManagingPopup`

### Trigger
- Right-click on a task in the TimeBasedOverview

### Features
- List all sub-tasks of the selected task
- Create a sub-task
- Delete a sub-task
- Edit sub-task name
- Edit sub-task color
- Edit sub-task start and end dates
- Assign or remove resources

---

## 8. Resource-Task View (Optional)

**Class name:** `ResourceTaskView`

### Purpose
- Display all tasks grouped by resource

### Features
- Show tasks assigned to each resource
- Identify overloaded resources (e.g., overlapping tasks in the same period)

---

## 9. HolidayProvider Interface

### Purpose
- Abstract the logic for determining holidays
- Allow flexible integration of different holiday data sources

### Responsibilities
- Determine whether a given date is a holiday
- Provide optional metadata about holidays (e.g., name)

### Interface Definition (Conceptual)
interface HolidayProvider {
bool isHoliday(Date date);
string getHolidayName(Date date); // optional
}


---

### 9.1 Expected Implementations

- `StaticHolidayProvider`
  - Uses predefined holiday data (e.g., JSON or configuration file)

- `LocaleBasedHolidayProvider`
  - Determines holidays based on system locale

- `RemoteHolidayProvider`
  - Retrieves holiday data from an external API

---

### 9.2 Integration with TimeBasedOverview

- `TimeBasedOverview` must depend on `HolidayProvider` via dependency injection
- During grid rendering:
  - For each day cell:
    - Call `isHoliday(date)`
    - If true → apply holiday background style (gray)
- Holiday name may be displayed as tooltip (optional)

---

## 10. Notes and Assumptions

- Time is handled using real calendar dates (day precision)
- Tasks must have a defined start date and end date
- The UI may group days into weeks/months for readability, but the smallest unit is one day
- Holiday data source should be configurable
- Visual interactions such as drag-and-drop resizing or moving tasks may be added in future versions