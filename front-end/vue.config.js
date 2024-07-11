const { defineConfig } = require('@vue/cli-service')
module.exports = defineConfig({
  transpileDependencies: true
})

module.exports = {
  devServer: {
    before: function(app) {
      app.get('/api/conversations/messages', function(req, res) {
        res.json([
          { id: 1, name: "Alice", email: "alice@example.com" },
          { id: 2, name: "Bob", email: "bob@example.com" }
        ]);
      });
    }
  }
}
