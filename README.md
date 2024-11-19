# Graph Theory But Game

![Project Status](https://img.shields.io/badge/Status-Complete-green?logoColor=%23A8E4A0) ![Project Version](https://img.shields.io/badge/Version-1.0.0-blue) ![Project License](https://img.shields.io/badge/License-MIT-blue) ![Contributors](https://badgen.net/github/contributors/PedroVillasBoas/GraphTheoryButGame) 

![](https://api.visitorbadge.io/api/VisitorHit?user=pedrovillasboas&repo=graphtheorybutgame&countColor=%2300BFFF)

# 🎢 Grafudos

Welcome to **Grafudos**! 🎉 An interactive, fun, and dynamic way to visualize and play with graphs right inside Unity. Whether you're a graph theory enthusiast, a computer science student, or just someone who loves nodes and edges bouncing around, this project is for you!

## 🚀 Features

- **Dynamic Graph Creation**: Add or remove vertices and edges on the fly.
- **Directed & Undirected Graphs**: Toggle between graph types effortlessly.
- **Weighted Edges**: Assign weights to edges and visualize weighted graphs.
- **Physics-Based Simulation**: Watch your graph come to life with physics!
- **Real-Time Physics Adjustment**: Adjust physics parameters with sliders and see immediate effects.
- **Toggle Physics On/Off**: Pause and resume the physics simulation at will.
- **Vertex Dragging**: Grab vertices and move them around the canvas.
- **Shortest Path Calculation**: Find and highlight the shortest path between two vertices.
- **Vertex Degree Information**: Get degrees of vertices, including in-degree and out-degree for directed graphs.
- **Adjacency Checking**: Check if two vertices are adjacent.
- **Graph Order**: See the amount of vertices at all times!
- **Graph Size**: Check how many vertices you can create!
- **Camera Control**: Pan and zoom the camera within bounds.
- **Smart Vertex Spawning**: Vertices spawn within specified areas, preferring ground over water.
- **Copy Vertex ID**: Right-click a vertex to copy its ID to your clipboard.

## 🎮 Getting Started

### Prerequisites

- Unity 2020.3 or later.

### Installation

1. **Clone the Repository**

   ```bash
   git clone https://github.com/PedroVillasBoas/GraphTheoryButGame.git
   ```

2. **Open in Unity**

   - Launch Unity Hub.
   - Click on "Open" and navigate to the cloned repository folder.

3. **Play**

   - Press the Play button in Unity to start the application.

## 🕹️ How to Use

### Adding Vertices

- **Automatic ID**: Click the "Add Vertex" button without entering an ID to generate a random vertex.
- **Custom ID**: Enter a unique integer ID in the vertex ID field and click "Add Vertex".

### Removing Vertices

- Enter the ID of the vertex you wish to remove and click "Remove Vertex".

### Adding Edges

- Enter the IDs of two existing vertices in the edge fields.
- Enter a weight if the graph is weighted.
- Click "Add Edge" to connect them.

### Removing Edges

- Enter the IDs of the two vertices connected by the edge.
- Click "Remove Edge" to delete the edge.

### Adjusting Physics

- Use the sliders to adjust:
  - **Center Force**: Pulls vertices towards the center.
  - **Repel Force**: Pushes vertices away from each other.
  - **Link Force**: Controls the stiffness of the edges.
- Click "Toggle Physics" to pause/resume the simulation.

### Dragging Vertices

- Click and hold a vertex to drag it around.
- Release the mouse button to drop the vertex.

### Shortest Path

- Enter the start and end vertex IDs.
- Click "Find Shortest Path" to calculate and highlight the path.

### Vertex Degree

- Enter a vertex ID and click "Get Degree" to see its degree.
- For directed graphs, in-degree and out-degree are displayed.

### Adjacency Check

- Enter two vertex IDs.
- Click "Check Adjacency" to see if they're connected.

### Camera Controls

- **Pan**: Click and hold the middle mouse button to move around.
- **Zoom**: Use the scroll wheel to zoom in and out.
- **Bounds**: Camera movement is constrained within the watery bounds—no escaping into the void!

### Copying Vertex ID

- Right-click on a vertex to copy its ID to your clipboard.
- Paste it anywhere—send it to a friend or keep it for your records!

## 🌊 The Environment

The graph resides in a world divided into three layers:

1. **Water**: The watery abyss. Vertices can spawn here but prefer land.
2. **Ground**: Solid ground where vertices feel most at home.
3. **Props**: Decorative elements to make the graph feel alive.

Vertices prefer to spawn on the ground but will venture into the water if necessary. Don't worry; they can swim! 🏊‍♂️

## ⚙️ Behind the Scenes

### Centralized Physics

All physics calculations are centralized in the `GraphManager`. This approach ensures efficient computations and smooth simulations, even when handling large graphs.

### Real-Time Physics Adjustment

The physics sliders are wired directly to the simulation parameters. Adjusting them changes the behavior of the vertices in real-time—no need to restart!

### Drag-and-Drop

Dragging vertices uses Unity's event system. When you pick up a vertex, it becomes kinematic, ignoring physics forces until you drop it back into the world.

### Camera Constraints

The camera is smart! It knows the boundaries of the watery world and won't let you peek beyond the edges, no matter how hard you pan or zoom.

### Smart Spawning

When adding new vertices, the system searches for valid positions within the water tilemap, preferring ground tiles. This ensures your graph doesn't spill over into uncharted territories.

### Copy-Paste IDs

Right-clicking a vertex uses Unity's `GUIUtility.systemCopyBuffer` to copy the vertex ID. Handy for quickly referencing specific nodes.

## 🧩 Extensibility

Want to add new features? The code is organized and commented for easy navigation. Feel free to fork the repository and customize it to your heart's content!

## 🐞 Troubleshooting

- **Vertices Not Moving?** Make sure physics is enabled and that your physics parameters aren't set to zero.
- **Can't Add Edge?** Ensure both vertex IDs exist and aren't the same.
- **Camera Won't Pan?** Remember, panning is done with the middle mouse button, and the camera won't go beyond the water's edge.

## 🤝 Contributing

Contributions are welcome! Please open an issue or submit a pull request.

## 📜 License

This project is licensed under the MIT License.

## 🎉 Acknowledgment

This "game" was made for a project in College. I had fun while doing it. (: 

## Team

| [<img loading="lazy" src="https://avatars.githubusercontent.com/u/108764670?v=4" width=115><br><sub>Adriana Lúcia</sub>](https://github.com/Dricalucia) |  [<img loading="lazy" src="https://avatars.githubusercontent.com/u/107653834?v=4" width=115><br><sub>Bruna Carvalho</sub>](https://github.com/brunacarvalho202)  | [<img loading="lazy" src="https://avatars.githubusercontent.com/u/116602650?v=4" width=115><br><sub>Gislaine Reis</sub>](https://github.com/lainereis2002) |  [<img loading="lazy" src="https://avatars.githubusercontent.com/u/47667167?v=4" width=115><br><sub>Guilherme Oliveira</sub>](https://github.com/Juillerms) |  
| :---: | :---: | :---: | :---: |
| [<img loading="lazy" src="https://avatars.githubusercontent.com/u/116356964?v=4" width=115><br><sub>Mariane Fontes</sub>](https://github.com/Dricalucia) |  [<img loading="lazy" src="https://avatars.githubusercontent.com/u/47667167?v=4" width=115><br><sub>Pedro Villas Boas</sub>](https://github.com/PedroVillasBoas)  | [<img loading="lazy" src="https://avatars.githubusercontent.com/u/112591325?v=4" width=115><br><sub>Thiago de Araújo</sub>](https://github.com/tharaujo17)  |   |  


---
© 2024 - Grafudos
