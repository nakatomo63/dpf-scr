# dpf-scr
is Digital Photo Frame Screen Saver

## Target Platform
- .NET Framework 4.7.2
- Windows 10

## Update History

### ver.0.14.12 2020/01/23
* [Chg] 代表色を2色抽出して時計とカレンダーにそれぞれ採用するようにした。彩度を調整した。
* [Chg] configのSizeにショートカットキーを追加。タブオーダーを修正。
* [Chg] 時計の垂直位置の設定を追加。
* [Fix] スライダの表示値がロード時に更新されていなかったのを修正。
* [Fix] サブディスプレイがメインディスプレイよりも上にある（サブディスプレイの左上隅のY座標が負）のとき、時計が真ん中に来ない現象を修正した。
### ver.0.13.11 2020/01/16
* [Chg] 代表色抽出の精度を上げた。
* [Fix] 前のバージョンから設定が引き継がれない問題を対策した
### ver.0.12.10 2020/01/13
* [Chg] 曜日を大文字にした。曜日の大きさを変えた。行間に区切り線を追加した。
### ver.0.11.9  2020/01/13
* [Add] デジタル時計に曜日(Sun,Mon,...)を追加した。
### ver.0.10.8  2020/01/11
* [Add] デジタル時計表示を追加した。時計は7segment表示の各segmentをBezier曲線で描画する。
	* 毎秒になった瞬間に表示が変わるよう、更新タイミングを工夫している。
	* 画像の代表色を検出で時計を描画するよう（Automatic Color）にした。代表色はHが画像全体の中央値、Sは画像全体の平均値、Vは固定値。
* [Chg] フレームロードを別スレッドに移して画面更新をスムーズにした。
* [Fix] マウスを動かして終了するとき、最初にほんの少し動かしてしまうと、再度大きく動かしても終了しないことがあった。
### ver.0.07 2019/11/24
* [Add] Crop表示モードを追加。画面の横幅・縦幅に合わせて拡大縮小するが、はみ出した部分はcropする。画像の縦長・横長に応じて適切に切り替える。
* [Add] 額縁除去モードを追加。横長の画像限定。画像の左端・右端の列の色が全部同じ一色である場合、そのような列を左右から除去して表示する。
* [Chg] Shuffuleモードでもう少しうまく混ざるようにした。
* [Chg] レンズの実焦点距離と35mm換算焦点距離が一致する場合、後者を表示しないようにした。
* [Chg] 露出補正値が±1EVぴったりの時に1/1ではなく1と表示するようにした。
### ver.0.06    2019/11/21
* [Fix] SystemParametersInfoAのDLLimportの仕方がおかしかったのを修正（Windows10で動かない）
### ver.0.05    2011/12/04
* [Add] Resume slideshow when Enter key is pressed or timed out. Timeout is equal to screen saver's one.
* [Add] Change to next file when Left/PageUp and Right/PageDown key is pressed, and the slideshow is paused.
* [Chg] Shutter speed calculation.
### ver.0.04    2010/04/13
* [Fix] TopMost form property set to false
* [Add] Load previous version settings when upgraded.
* [Add] Image resolutional 'Size' in information area
* [Chg] Image 'Size' to 'Original Size'
### ver.0.03    2010/04/11
* [Fix] Too much RAM is consumed until GC starts its work.
### ver.0.02    2010/04/02
* [Fix] ExposureBiasValue GCD Calculation
* [Fix] Default File Name Filter set to '.*'
###ver.0.01    2010/04/02
* First Version
