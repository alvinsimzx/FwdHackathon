const dataContainer = d3.select('.upload-csv')

// Grabs file from file upload input, populates it into csv component
const readURL = input => {

  // Check if any files were selected
  if (input.files && input.files[0]) {

    $('.upload-text').text('Loading..');

    // List of categories + counter
    var categoryList = [];

    // Initialize JS file reader
    var reader = new FileReader();

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