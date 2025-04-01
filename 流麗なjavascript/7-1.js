function compareRobots(robot1, memory1, robot2, memory2) {
  const tasks = [];
  for (let i = 0; i < 100; i++) {
    tasks.push(randomPick(['A', 'B', 'C', 'D']));
  }

  const steps1 = runRobot(tasks, robot1, memory1);
  const steps2 = runRobot(tasks, robot2, memory2);

  console.log(`Robot 1: ${steps1} steps`);
  console.log(`Robot 2: ${steps2} steps`);
}
class VillageState {
  constructor(place, parcels) {
    this.place = place;
    this.parcels = parcels;
  }
  move(destination) {
    if (!roadGraph[this.place].includes(destination)) {
      return this;
    } else {
      let parcels = this.parcels.map(p => {
        if (p.place != this.place) return p;
        return {place: destination, address: p.address};
      }).filter(p => p.place != p.address);
      return new VillageState(destination, parcels);
    }
  }
}
class Robot {
  constructor(memory) {
    this.memory = memory;
  }
  move(state, memory) {
    return {direction: 'E', memory};
  }
}
function randomPick(array) {
  const choice = Math.floor(Math.random() * array.length);
  return array[choice];
}
function runRobot(tasks, robot, memory) {
  let state = {place: 0, parcels: tasks};
  for (let turn = 0;; turn++) {
    if (state.parcels.length == 0) {
      return turn;
    }
    const action = robot.move(state, memory);
    state = state.move(action.direction);
    memory = action.memory;
  }
}
compareRobots(Robot, [], Robot, []);