﻿// Rendering graph
const svg = d3.select('svg');

const width = +svg.attr('width');
const height = +svg.attr('height');

const render = data => {
  // value accesors
  const xValue = d => d.population;
  const yValue = d => d.country;

  // margin convention
  const margin = { top: 20, right: 20, bottom: 20, left: 80 };
  const innerWidth = width - margin.left - margin.right;
  const innerHeight = height - margin.top - margin.bottom;

  // domain accepts min and d3.max values of dataset
  // range accepts min and d3.max values of screen size
  const xScale = d3.scaleLinear()
    .domain([0, d3.max(data, xValue)])
    .range([0, innerWidth]);

  const yScale = d3.scaleBand()
    .domain(data.map(yValue))
    .range([0, innerHeight])
    .padding(0.1);

  // groups everything 
  const g = svg.append('g')
    .attr('transform', `translate(${margin.left}, ${margin.top})`)

  // const yAxis = d3.axisLeft(yScale);
  // yAxis(g.append('g'));
  g.append('g').call(d3.axisLeft(yScale));
  g.append('g').call(d3.axisBottom(xScale))
    .attr('transform', `translate(0, ${innerHeight})`);

  g.selectAll('rect').data(data)
    .enter().append('rect')
    .attr('y', d => yScale(yValue(d)))
    .attr('width', d => xScale(xValue(d)))
    .attr('height', yScale.bandwidth())
};

// Creates a delay
const timeout = ms => {
  return new Promise(resolve => setTimeout(resolve, ms));
}

// Initialize loading animation
const loading = async (loadingCheck) => {

  while (loadingCheck.isRunning) {
    console.log("running");

    await timeout(250);
    $('.upload-text').text('Loading..');
    await timeout(250);
    $('.upload-text').text('Loading...');
  }

  $('.upload-text').text('Computed!');
}

// Grabs file from file upload input, populates it into csv component
const readURL = input => {

  // Check if any files were selected
  if (input.files && input.files[0]) {

    // Variable for checking loading script
    let loadingCheck = { "isRunning": true }

    // Start async loading function
    loading(loadingCheck);

    // List of categories + counter
    let categoryList = [];

    // Initialize JS file reader
    let reader = new FileReader();

    // Console log each data after the reader loads
    reader.onload = e => {

      // Load the data using D3
      d3.csv(e.target.result).then(data => {

        // Compute the number of categories for each row
        data.forEach((row) => {
          let key = row.category;

          // If number doesn't exist, assign 1
          typeof categoryList[key] == "number"
            ? categoryList[key] += 1
            : categoryList[key] = 1;
        })
      });

      // Stop loading
      loadingCheck.isRunning = false;

      // Display graph of categories
      console.log(categoryList);
      //render(categoryList)

      // Write aggregated data to db

    }

    // Load the input file
    reader.readAsDataURL(input.files[0]);
  }
}

$("#fileupload").change(function () {
  readURL(this);
});