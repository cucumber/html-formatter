import MiniCssExtractPlugin from 'mini-css-extract-plugin'

export default {
  entry: './dist/src/main.js',
  module: {
    rules: [
      {
        test: /\.scss$/,
        use: [
          MiniCssExtractPlugin.loader,
          {
            loader: 'css-loader',
            options: {
              modules: {
                auto: true,
                namedExport: false,
              },
            },
          },
          'sass-loader',
        ],
      },
    ],
  },
  plugins: [
    new MiniCssExtractPlugin({
      filename: 'main.css',
    }),
  ],
  optimization: {
    // webpack's built-in CSS minifier (on by default from 5.110) downlevels our
    // oklch() colours to hex, which changes how reports render, so leave it off.
    minimize: {
      css: false,
    },
  },
}
