const fs = require("fs");
const { machine } = require("os");
const path = require("path");

const INPUT_FILE = path.join(__dirname, "../input.txt");

fs.readFile(INPUT_FILE, "utf-8", (err, input) => {
  if (err) {
    console.error(`Error reading file from path ${INPUT_FILE}:`, err);
    process.exit(1);
  }

  const machines = input.split("\n\n").map((m) => m.match(/\d+/g).map(Number));

  function tokens([aX, aY, bX, bY, pX, pY]){
    // aX * a + bX * b = pX
    // aY * a + bY * b = pY
    const a = (pX * bY - pY * bX) / (aX * bY - aY * bX);
    const b = (aX * pY - aY * pX) / (aX * bY - aY * bX);

    return Number.isInteger(a) && Number.isInteger(b) ? a * 3 + b : 0;
  }

  const e = machines.reduce((acc, machine) =>
  {
    machine[4] = machine[4] + Math.pow(10, 13);
    machine[5] = machine[5] + Math.pow(10, 13);
    return acc + tokens(machine);
  }, 0);


  console.log(e);

});