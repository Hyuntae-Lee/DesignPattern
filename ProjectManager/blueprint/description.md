1. This is an application to manage multi-projects sharing resources(ex: developers).
2. The main screen should be composed with three panels of ProjectView, TimelineView, and ResourceView.
3. ProjectView
   - Should be located at the left side of the screen and have fixed width.
   - Should allow the user to run following functions.
     - Show and hide this panel.
     - Create and remove project.
     - Edit color and name of existing projects.
     - Edit color and name of existing tasks in projects.
4. ResourceView
   - Should be located at the right side of the screen and have fixed width.
     - Should allow the user to run following functions.
     - Show and hide this panel.
     - Create and remove resource.
     - Edit color and name of existing resource.
5. TimelineView
   - Should be located at the center of the screen and fill all space except ProjectView and ResourceView.
   - It has three layers of TimeLineLayer, PathEmphasisLayer, and TaskLayer.
   - TimeLineLayer
     - Dispalys time line by weeks with grid view.
     - Rough layout is following.
       |                  2026                 | <-- Year
       |---------------------------------------|
       |  Jan  |  Feb  |  Mar  | April |  May  | <-- Month
       |---------------------------------------|
       | | | | | | | | | | | | | | | | | | | | | <-- Weeks
       |---------------------------------------|
       | | | | | | | | | | | | | | | | | | | | |
       | | | | | | | | | | | | | | | | | | | | |
       | | | | | | | | | | | | | | | | | | | | |
       | | | | | | | | | | | | | | | | | | | | |
       | | | | | | | | | | | | | | | | | | | | |
       | | | | | | | | | | | | | | | | | | | | |
     - TaskLayer
       - Displays projects and tasks by overlaying TimeLineLayer.
       - Rough layout is following.
       |                  2026                 | <-- Year
       |---------------------------------------|
       |  Jan  |  Feb  |  Mar  | April |  May  | <-- Month
       |---------------------------------------|
       | | | | | | | | | | | | | | | | | | | | | <-- Weeks
       |---------------------------------------|
       +---------------------------------------+
       |               Project1                |
       +---------------------------------------+
       | +-------------+ | | | | | | | | | | | |
       | |    Task1    | | | | | | | | | | | | |
       | +-------------+ | | | | | | | | | | | |
       | | | | | | | | +---------------+ | | | |
       | | | | | | | | |     Task2     | | | | |
       | | | | | | | | +---------------+ | | | |
       | | | | | | | | | | | | | | | | | | | | |
     - PathEmphasisLayer
       - Highlight specific areas of the grid.
       - The application should allow the user to add/remove and edit the areas.
         

