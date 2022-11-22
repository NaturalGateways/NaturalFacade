import { Component } from '@angular/core';

export class LayoutTestItem {
  layoutId: string | undefined;
  layoutName: string | undefined;
}

@Component({
  selector: 'app-layouts',
  templateUrl: './layouts.component.html',
  styleUrls: ['./layouts.component.css']
})
export class LayoutsComponent {

  layouts: any = [];

  constructor() {
    var item1: LayoutTestItem = new LayoutTestItem();
    item1.layoutId = "alpha";
    item1.layoutName = "Alpha";
    var item2: LayoutTestItem = new LayoutTestItem();
    item2.layoutId = "beta";
    item2.layoutName = "Beta";
    var item3: LayoutTestItem = new LayoutTestItem();
    item3.layoutId = "gamma";
    item3.layoutName = "Gamma";
    this.layouts.push(item1);
    this.layouts.push(item2);
    this.layouts.push(item3);
  }
}
