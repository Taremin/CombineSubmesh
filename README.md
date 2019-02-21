# Combine Submesh

同じマテリアルを参照しているサブメッシュを一つのサブメッシュにまとめる Unity エディタ拡張です。

## 使い方

1. ヒエラルキーで対象となるアバターを選択
2. 右クリックメニューから「CombineSubmesh」を選択
3. CombineSubmeshタブ(ウィンドウ)から「Combine Submesh」ボタンを押す

選択したオブジェクト以下に含まれる `SkinnedMeshRenderer` の書き換えを行います。
(子や孫オブジェクトのものも書き換えられるということです。)
元のメッシュのある場所と同じところに `*.OptimizedMesh` というファイルが作られますが、これがサブメッシュの結合後のメッシュデータです。

## ライセンス

[MIT](./LICENSE)
