const dataContainer = d3.select('.upload-csv')

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
    let loadingCheck = { "isRunning" : true}

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


      // Write aggregated data to db
      console.log(categoryList);
    }

    // Load the input file
    reader.readAsDataURL(input.files[0]);
  }
}

$("#fileupload").change(function () {
  readURL(this);
});