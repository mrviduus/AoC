const fs = require("fs");
const path = require("path");

const INPUT_FILE = path.join(__dirname, "../input.txt");

fs.readFile(INPUT_FILE, "utf-8", (err, input) => {
  if (err) {
    console.error(`Error reading file from path ${INPUT_FILE}:`, err);
    process.exit(1);
  }
  const data = input.split("\n").map((x) => x.split("").map(Number));

  /*Iterates through the 2D grid (input).
Identifies all positions where the value is 0.
Stores these positions as [row, column] pairs in the zeros array.  */
let zeros = []
for (let i = 0; i < data.length; i++)
  for (let j = 0; j < data[i].length; j++)
    if (data[i][j] == 0) zeros.push([i, j])

/*Takes the position (pos) of a cell in the grid as input.
Looks for adjacent cells (up, down, left, right) that contain the next value in the sequence (current + 1).
The ?. ensures no error occurs if an index is out of bounds.
Returns an array of positions of these "next cells."  */
function findNext(pos) {
  let current = data[pos[0]][pos[1]],
    next = []

  if (data[pos[0] + 1]?.[pos[1]] == current + 1)
    next.push([pos[0] + 1, pos[1]])
  if (data[pos[0] - 1]?.[pos[1]] == current + 1)
    next.push([pos[0] - 1, pos[1]])
  if (data[pos[0]][pos[1] + 1] == current + 1) next.push([pos[0], pos[1] + 1])
  if (data[pos[0]][pos[1] - 1] == current + 1) next.push([pos[0], pos[1] - 1])

  return next
}


/*Initialization:
For each zero in zeros, start a trail search.
Initialize next with the position of the current zero.

Trail Traversal:
While there are positions to explore (next.length > 0):
For each position in next:
If the current cell value is 9, the trail is complete, so increment trailheads.
Otherwise, use findNext to find adjacent cells that continue the trail.
Collect all new positions found in found and update next to continue traversal.

Termination:
The loop stops when no more valid positions are found (next.length === 0).  */

let trailheads = 0
zeros.forEach((pos) => {
  let next = [pos]
  while (next.length) {
    let found = []
    next.forEach((pos) => {
      if (data[pos[0]][pos[1]] == 9) trailheads++
      else found.push(...findNext(pos))
    })
    // found.forEach((pos, i) => {
    //   for (let j = i + 1; j < found.length; j++)
    //     if (found[j][0] == pos[0] && found[j][1] == pos[1]) found[j] = []
    // })
    next = found.filter((x) => x.length)
  }
})

console.log(trailheads);

});