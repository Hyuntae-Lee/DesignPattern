[Application Overview]

This application manages multiple projects that share resources (e.g., developers).
The UI consists of three main panels: ProjectView, TimelineView, and ResourceView.

---

[Main Layout]

* The main window uses a horizontal layout with three panels:

  1. ProjectView

     * Positioned on the left
     * Fixed width: 250px
     * Can be collapsed (width becomes 0)

  2. TimelineView

     * Positioned in the center
     * Fills all remaining horizontal space

  3. ResourceView

     * Positioned on the right
     * Fixed width: 250px
     * Can be collapsed (width becomes 0)

---

[Timeline System]

* The timeline is based on a horizontal time axis.

[TimeScale Rules]

* Unit: 1 week

* weekWidth: 40 pixels

* Date to X-coordinate conversion:
  x = (date - startDate).TotalDays / 7 * weekWidth

* All layers must use the same TimeScale instance.

---

[Vertical Layout Rules]

* Each Project occupies one row
* Row height: 60px
* Tasks are rendered inside the corresponding Project row

---

[Timeline Rendering Layers]

Rendering order (bottom to top):

1. TimeLineLayer (background)
2. PathEmphasisLayer (middle)
3. TaskLayer (top)

---

[TimeLineLayer Specification]

* Renders three horizontal sections:

  1. Year header
  2. Month header
  3. Week grid

* Week grid:

  * Each column width: 40px
  * Vertical lines for each week

---

[TaskLayer Specification]

* For each Project:
  y = projectIndex * rowHeight

* Render a project row background (full width)

* For each Task:
  x = dateToX(beginDate)
  width = dateToX(endDate) - x
  y = projectRowY + padding

* Each Task is rendered as a rectangle using its color

---

[PathEmphasisLayer Specification]

* For each EmphasisArea:
  x = dateToX(startDate)
  width = dateToX(endDate) - x
  height = full timeline height

* Render as semi-transparent overlay

---

[Selection and Interaction Model]

* Maintain a SelectionState with:

  * selectedProject
  * selectedTask

* Interaction rules:

  * Clicking a Project sets selectedProject
  * Clicking a Task sets selectedTask
  * Editing operations apply to the selected item

---

[ProjectView Behavior]

* Allows:

  * Create Project
  * Delete Project
  * Edit Project (name, color)
  * Edit Task (name, color)
  * Show / Hide panel

---

[ResourceView Behavior]

* Allows:

  * Create Resource
  * Delete Resource
  * Edit Resource (name, color)
  * Show / Hide panel

---

[TimelineView Behavior]

* Displays:

  * All projects and tasks
  * Emphasis areas

* Provides:

  * Visual feedback for selected items
  * Layered rendering (grid, emphasis, tasks)

---

[Implementation Rules]

* All model classes must support change notification (e.g., INotifyPropertyChanged)

* AppState acts as the central data store (singleton for now)

* Views must not contain business logic

  * Only rendering and user interaction

* TimeScale must be used consistently across all rendering logic

* Avoid hardcoding layout values outside defined rules

---

[Rendering Summary]

* Horizontal positioning is always derived from TimeScale
* Vertical positioning is based on project index and fixed row height
* Layers must be rendered in strict order to ensure correct visuals

---
