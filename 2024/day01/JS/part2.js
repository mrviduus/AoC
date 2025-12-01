const fs = require('fs');
const path = require('path');

const DECIMAL_BASE = 10;
const INPUT_FILE = path.join(__dirname, './../input.txt');

function readInputFile(filePath) {
  try {
    return fs.readFileSync(filePath, 'utf-8').split('\n');
  } catch (error) {
    console.error(`Error reading file from path ${filePath}:`, error);
    process.exit(1);
  }
}

function parseLine(line) {
  const parts = line.trim().split(/\s+/);
  if (parts.length === 2 && !isNaN(parts[0]) && !isNaN(parts[1])) {
    return [parseInt(parts[0], DECIMAL_BASE), parseInt(parts[1], DECIMAL_BASE)];
  }
  return null;
}

/**
 * Calculates the similarity score based on the left and right values.
 * @param {number[]} left - The array of left values.
 * @param {Object} right - The object containing the count of right values.
 * @returns {number} - The calculated similarity score.
 */
function calculateSimilarityScore(left, right) {
  let similarityScore = 0;
  left.forEach(l => {
    similarityScore += l * (right[l] || 0);
  });
  return similarityScore;
}

// Main execution
const inputLines = readInputFile(INPUT_FILE);

const left = [];
const right = {};

inputLines.forEach(line => {
  const parsed = parseLine(line);
  if (parsed) {
    const [leftValue, rightValue] = parsed;
    left.push(leftValue);
    right[rightValue] = (right[rightValue] || 0) + 1;
  }
});

const similarityScore = calculateSimilarityScore(left, right);

console.log(similarityScore);